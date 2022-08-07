using System.Collections.Generic;
using GameSave;
using TMPro;
using UnityEngine;

namespace GameUI
{
    public class ScoreBoard : IPersistantObject
    {
        public static ScoreBoard Instance { get; private set; }
        public ScorePlate highScorePrefab;
        public GameObject BoardBackground;
        public GameObject ScoreBoardPanel;
        public float PopScaleSpeed;
        private List<ScorePlate> scorePlates;
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
            
            if (scorePlates == null) 
                scorePlates = new List<ScorePlate>();
        }
        public override void Save(GameDataWriter writer)
        {
            int count = scorePlates.Count;
            writer.Write(count);
            for (int i = 0; i < count; i++)
            {
                scorePlates[i].Save(writer);
            }
        }
        public override void Load(GameDataReader reader)
        {
            ResetChecks();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ScorePlate sP = Instantiate(highScorePrefab, scorePlatesParent);
                sP.Load(reader);
            }
        }
        public int HighScore()
        {
            int result = 0;
            for (int i = 0; i < scorePlates.Count; i++)
            {
                if (scorePlates[i].Score > result)
                    result = scorePlates[i].Score;
            }
            return result;
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

            scorePlates.Add(sP);
        }
        private void ResetChecks()
        {
            for (int i = 0; i < scorePlates.Count; i++)
            {
                ScorePlate sP = scorePlates[i];
                scorePlates.RemoveAt(i);
                Destroy(sP.gameObject);
            }
        }
    }
}
