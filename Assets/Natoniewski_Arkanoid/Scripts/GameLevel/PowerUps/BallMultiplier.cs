using System;

namespace GameLevel.PowerUps
{
    public class BallMultiplier : PowerUp
    {
        public override int scoreToGet { get; set; } = 10;
        public static event Action OnBallsMultiply;
        public override void Activate()
        {
            base.Activate();
            OnBallsMultiply?.Invoke();
            Destroy(gameObject);
        }
    }
}