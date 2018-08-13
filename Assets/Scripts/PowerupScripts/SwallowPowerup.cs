using UnityEngine;

public class SwallowPowerup : PowerupBase
{
    protected override void BallActivate(GameObject ball)
    {
        Destroy(ball);
    }
}