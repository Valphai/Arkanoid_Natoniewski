using System;
using GameSave;
using UnityEngine;

namespace GameLevel.Bricks
{
    public abstract class Brick : IPersistantObject
    {
        private Color color;
        private Sprite sprite;
        [SerializeField] private int brickDestroyScore;
        public static event Action<int> OnBrickDestroyed;

        public void Destroy()
        {
            OnBrickDestroyed?.Invoke(brickDestroyScore);
        }
    }
}
