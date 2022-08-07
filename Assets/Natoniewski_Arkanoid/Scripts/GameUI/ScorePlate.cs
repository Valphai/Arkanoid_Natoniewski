using GameSave;
using TMPro;
using UnityEngine;

namespace GameUI
{
    public class ScorePlate : IPersistantObject
    {
        public int Score { get; private set; }
        private string plateName;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        public void SetScore(string name, int score)
        {
            this.plateName = name;
            this.Score = score;
            nameText.text = name;
            scoreText.text = score.ToString();
            ScoreManager.Instance.SetHighScore(score);
        }
        public override void Save(GameDataWriter writer)
        {
            writer.Write(plateName);
            writer.Write(Score);
        }
        public override void Load(GameDataReader reader)
        {
            string name = reader.ReadString();
            int score = reader.ReadInt();
            SetScore(name, score);
        }
    }
}