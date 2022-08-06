using UnityEngine;

namespace GameUI
{
    public class LifeTracker : MonoBehaviour
    {
        public static LifeTracker Instance { get; private set; }
        [SerializeField] private GameObject[] lifeVisual;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        public void UpdateLife(int currentGameLife)
        {
            for (int i = 0; i < lifeVisual.Length; i++)
            {
                lifeVisual[i].SetActive(false);
            }
            for (int i = 0; i < currentGameLife; i++)
            {
                lifeVisual[i].SetActive(true);
            }
        }
    }
}