using UnityEngine;

namespace GameLevel
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Arkanoid_Natoniewski/LevelData")]
    public class LevelData : ScriptableObject
    {
        public float BrickWidth = .5f;
        public float BrickHeight = .25f;
        public int LevelWidth = 13;
        public int LevelHeight = 15;
    }
}