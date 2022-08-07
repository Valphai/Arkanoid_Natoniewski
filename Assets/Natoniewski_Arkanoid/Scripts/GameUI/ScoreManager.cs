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
        [SerializeField] private ScoreBoard scoreBoard;
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
        }
        private void OnDisable()
        {
            Brick.OnBrickDestroyed -= IncreaseScore;
        }
        public int CurrentScore() => currentScore;
        public void IncreaseScore(Brick b)
        {
            currentScore += b.GetScore();
            SetScoreText();
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
        public void StartNewGame()
        {
            currentScore = 0;
            highScore = scoreBoard.HighScore();
            SetScoreText();
        }
        public void SetHighScore(int value)
        {
            if (value > highScore)
            {
                highScore = value;
                SetScoreText();
            }
        }
        private void SetScoreText()
        {
            scoreText.text = currentScore.ToString();
            highScoreText.text = highScore.ToString();
        }
    }
}
