using TMPro;
using UnityEngine;

namespace GameUI
{
    public class ScoreBoard : MonoBehaviour
    {
        public ScorePlate highScorePrefab;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Transform scorePlatesParent;
        [SerializeField, HideInInspector] private int score;
        [SerializeField, HideInInspector] private bool submitedScore;
        public void Activate()
        {
            score = ScoreManager.Instance.CurrentScore();
            scoreText.text = score.ToString();
        }
        public void DeActivate()
        {
            submitedScore = false;
        }
        public void SubmitYourScore()
        {
            string name = inputField.text;
            if (name == null || submitedScore) return;

            ScorePlate sP = Instantiate(highScorePrefab, scorePlatesParent);

            sP.SetScore(name, score);
            submitedScore = true;
        }
    }
}
