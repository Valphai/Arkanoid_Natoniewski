using System;
using GameLevel.PowerUps;

public class VausLaser : PowerUp
{
    public override int scoreToGet { get; set; } = 15;

    public static event Action OnGrabLaserPower;
    public override void Activate()
    {
        base.Activate();
        OnGrabLaserPower?.Invoke();
        Destroy(gameObject);
    }
}