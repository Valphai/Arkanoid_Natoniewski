using UnityEngine;

namespace GameLevel
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Arkanoid_Natoniewski/LevelData")]
    public class LevelData : ScriptableObject
    {
        public const float YOffset = -1.5f;
        public const float XOffset = -2.84f;
        private const float PixelsPerUnit = 14f;
        public const float BrickWidth = 7f / PixelsPerUnit;
        public const float BrickHeight = 4f / PixelsPerUnit;
        public const int LevelWidth = 13;
        public const int LevelHeight = 17;
        public const string FactoryName = "Factory";
        public Color[] BrickColors;
    }
}