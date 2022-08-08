using UnityEngine;

namespace GameLevel
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Arkanoid_Natoniewski/LevelData")]
    public class LevelData : ScriptableObject
    {
        public const float YOffset = -LevelHeight / 2 * BrickHeight;
        public const float XOffset = -(LevelWidth / 2 * BrickWidth + BrickWidth / 2);
        private const float PixelsPerUnit = 14f;
        public const float BrickWidth = 7f / PixelsPerUnit;
        public const float BrickHeight = 4f / PixelsPerUnit;
        public const int LevelWidth = 13; // assumed to be odd
        public const int LevelHeight = 17; // assumed to be odd
        public const string FactoryName = "Factory";
        public Color[] BrickColors;
    }
}