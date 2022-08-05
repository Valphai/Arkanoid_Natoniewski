using TMPro;
using UnityEngine;

namespace GameUI
{
    public class ScorePlate : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        public void SetScore(string name, int score)
        {
            nameText.text = name;
            scoreText.text = score.ToString();
        }
    }
}