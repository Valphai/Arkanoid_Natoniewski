using GameLevel.Bricks;
using GameSave;
using TMPro;
using UnityEngine;

namespace GameUI
{
    public class ScoreManager : IPersistantObject
    {
        public static ScoreManager Instance { get; private set; }
        [SerializeField] private int currentScore;
        [SerializeField] private int highScore;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        private void OnEnable()
        {
            Brick.OnBrickDestroyed += IncreaseScore;
            currentScore = 0;
            highScore = 0;
            SetScoreText();
        }
        private void OnDisable()
        {
            Brick.OnBrickDestroyed -= IncreaseScore;
        }
        public void IncreaseScore(int val)
        {
            currentScore += val;
        }
        public override void Save(GameDataWriter writer)
        {
            writer.Write(currentScore);
            writer.Write(highScore);
        }
        public override void Load(GameDataReader reader)
        {
            currentScore = reader.ReadInt();
            highScore = reader.ReadInt();
            SetScoreText();
        }

        private void SetScoreText()
        {
            scoreText.text = currentScore.ToString();
            highScoreText.text = highScore.ToString();
        }
    }
}
