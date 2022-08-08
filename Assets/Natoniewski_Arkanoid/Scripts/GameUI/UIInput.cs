using System;
using TMPro;
using UnityEngine;

namespace GameUI
{
    public class UIInput : MonoBehaviour
    {
        public static UIInput Instance { get; private set; }
        public bool FileExists { get; set; }
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private TextMeshProUGUI popUpText;
        [SerializeField] private float popUpTime = 2f;
        public static event Action OnNewGame;
        public static event Action OnResumeGame;
        public static event Action OnPauseGame;
        public static event Action OnGameSave;
        public static event Action OnGameLoad;
        public static event Action OnLoadRequest;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        public void OnPlayUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!pauseMenu.activeSelf)
                {
                    OnPauseGame?.Invoke();
                    pauseMenu.SetActive(!pauseMenu.activeSelf);
                }
            }
        }
        public void Home() => mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
        public void ResumeGame() => OnResumeGame?.Invoke();
        public void StartNewGame() => OnNewGame?.Invoke();
        public void QuitGame() => Application.Quit();
        public void Save()
        {
            TweenText("Saved!");
            OnGameSave?.Invoke();
        }
        public void Load()
        {
            OnLoadRequest?.Invoke();
            if (!FileExists) return;
            
            Home();
            TweenText("Loaded!");
            OnGameLoad?.Invoke();
        }
        private void TweenText(string text)
        {
            Vector2 initPosition = popUpText.gameObject.transform.position;
            popUpText.gameObject.SetActive(true);
            popUpText.text = text;
            popUpText.gameObject.transform.localScale = Vector3.zero;
            LeanTween.scale(popUpText.gameObject, Vector3.one, popUpTime).setOnComplete(
                () => LeanTween.moveY(popUpText.gameObject, Screen.height * 1.5f, popUpTime * .5f).setOnComplete(
                    () => {
                        popUpText.gameObject.SetActive(false);
                        popUpText.transform.position = initPosition;
                    }
                )
            );
        }
    }
}