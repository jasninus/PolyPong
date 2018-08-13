using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilityPowerup : PowerupBase
{
    private MeshRenderer ballRend;

    protected override void BallActivate(GameObject ball)
    {
        ballRend = ball.GetComponent<MeshRenderer>();

        ballRend.enabled = false;
    }

    protected override void Revert()
    {
        ballRend.enabled = true;
    }
}