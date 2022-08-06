using System;
using UnityEngine;

namespace GameUI
{
    public class MenuButtons : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuPanel;
        public static event Action OnNewGame;
        public static event Action OnGameSave;
        public static event Action OnGameLoad;
        public void Home() => mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
        public void StartNewGame() => OnNewGame?.Invoke();
        public void Save() => OnGameSave?.Invoke();
        public void Load() => OnGameLoad?.Invoke();
    }
    
}