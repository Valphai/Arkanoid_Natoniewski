using System;

namespace GameLevel.PowerUps
{
    public class BallMultiplier : PowerUp
    {
        public static event Action OnBallsMultiply;
        public override void Activate()
        {
            OnBallsMultiply?.Invoke();
            Destroy(gameObject);
        }
    }
}