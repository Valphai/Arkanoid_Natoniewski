using GameLevel.Bricks;
using GameSave;
using UnityEngine;

namespace GameLevel
{
    public class BrickSlot : IPersistantObject
    {
        public int X, Y;
        private Brick brick;
        private BrickSlot[] neighbours; // 8

        public BrickSlot(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            neighbours = new BrickSlot[8];
        }

        public BrickSlot NextIn(Direction dir) => neighbours[(int)dir];
        public bool IsFilled() => brick != null;
        public void FillWith(Brick b)
        {
            brick = b;
            brick.transform.position = new Vector2(X, Y);
        }
        public void SetNeighbor(Direction direction, BrickSlot b) 
        {
            neighbours[(int)direction] = b;
            b.neighbours[(int)direction.Opposite()] = this;
        }
    }
}
