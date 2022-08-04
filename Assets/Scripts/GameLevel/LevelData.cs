using UnityEngine;

namespace GameLevel
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Arkanoid_Natoniewski/LevelData")]
    public class LevelData : ScriptableObject
    {
        public const float YOffset = -1f;
        public const float XOffset = -3.14f;
        public const float BrickWidth = .5f;
        public const float BrickHeight = .25f;
        public const int LevelWidth = 14;
        public const int LevelHeight = 17;
    }
}