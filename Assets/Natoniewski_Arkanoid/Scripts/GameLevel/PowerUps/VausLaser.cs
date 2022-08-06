using System;
using GameLevel.PowerUps;

public class VausLaser : PowerUp
{
    public static event Action OnGrabLaserPower;
    public override void Activate()
    {
        OnGrabLaserPower?.Invoke();
        Destroy(gameObject);
    }
}