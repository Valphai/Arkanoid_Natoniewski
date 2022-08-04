using GameLevel.Bricks;
using UnityEngine;

namespace GameLevel
{
    public class BrickSlot
    {
        private Brick brick;
        private BrickSlot[] neighbours; // 8
        private readonly float x, y;

        public BrickSlot(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public void Awake() => neighbours = new BrickSlot[8];
        public BrickSlot NextIn(Direction dir) => neighbours[(int)dir];
        public Vector2 Position() => new Vector2(x, y);
        public bool IsFilled() => brick != null;
        public void FillWith(Brick b)
        {
            brick = b;
            brick.transform.position = new Vector2(x, y);
        }
        public void SetNeighbor(Direction direction, BrickSlot b) 
        {
            neighbours[(int)direction] = b;
            b.neighbours[(int)direction.Opposite()] = this;
        }
        private void OnDrawGizmos()
        {
            var t = new Vector2(x, y);
            Gizmos.color = Color.white;

            Gizmos.DrawLine(
                new Vector3(t.x - 0.5f * LevelData.BrickWidth, t.y - 0.5f * LevelData.BrickHeight),
                new Vector3(t.x - 0.5f * LevelData.BrickWidth, t.y + 0.5f * LevelData.BrickHeight)
            );
            Gizmos.DrawLine(
                new Vector3(t.x - 0.5f * LevelData.BrickWidth, t.y - 0.5f * LevelData.BrickHeight),
                new Vector3(t.x + 0.5f * LevelData.BrickWidth, t.y - 0.5f * LevelData.BrickHeight)
            );
            Gizmos.DrawLine(
                new Vector3(t.x + 0.5f * LevelData.BrickWidth, t.y - 0.5f * LevelData.BrickHeight),
                new Vector3(t.x + 0.5f * LevelData.BrickWidth, t.y + 0.5f * LevelData.BrickHeight)
            );
            Gizmos.DrawLine(
                new Vector3(t.x - 0.5f * LevelData.BrickWidth, t.y + 0.5f * LevelData.BrickHeight),
                new Vector3(t.x + 0.5f * LevelData.BrickWidth, t.y + 0.5f * LevelData.BrickHeight)
            );
        }
    }
}
