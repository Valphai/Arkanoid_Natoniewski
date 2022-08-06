using TMPro;
using UnityEngine;

namespace GameUI
{
    public class ScoreBoard : MonoBehaviour
    {
        public static ScoreBoard Instance { get; private set; }
        public ScorePlate highScorePrefab;
        public GameObject BoardBackground;
        public GameObject ScoreBoardPanel;
        public float PopScaleSpeed;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Transform scorePlatesParent;
        [SerializeField, HideInInspector] private int score;
        [SerializeField, HideInInspector] private bool submitedScore;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        public void Activate()
        {
            score = ScoreManager.Instance.CurrentScore();
            scoreText.text = score.ToString();
            BoardBackground.SetActive(true);
            ScoreBoardPanel.transform.localScale = Vector3.zero;
            LeanTween.scale(ScoreBoardPanel, Vector3.one, PopScaleSpeed);
        }
        public void DeActivate()
        {
            submitedScore = false;
            BoardBackground.SetActive(false);

        }
        public void SubmitYourScore()
        {
            string name = inputField.text;
            if (name == "" || submitedScore) return;

            ScorePlate sP = Instantiate(highScorePrefab, scorePlatesParent);

            sP.SetScore(name, score);
            submitedScore = true;
        }
    }
}
