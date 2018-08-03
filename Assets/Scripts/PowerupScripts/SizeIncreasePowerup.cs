using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeIncreasePowerup : PowerupBase
{
    [SerializeField] private float playerSizeIncrease, ballSizeIncrease, sizeChangeDuration;
    private float lerpStartTime, originalSize;

    private bool lerpBigger, lerpSmaller;

    private Transform enlargedPlayer, enlargedBall;

    protected override void PlayerActivate(Player lastPlayerHit)
    {
        enlargedPlayer = lastPlayerHit.transform.GetChild(0);
        //enlargedPlayer.localScale = new Vector3(enlargedPlayer.localScale.x + playerSizeIncrease, enlargedPlayer.localScale.y, enlargedPlayer.localScale.z);
        originalSize = enlargedPlayer.localScale.x;
        lerpBigger = true;
        lerpStartTime = Time.time;
    }

    protected override void BallActivate(GameObject ball)
    {
        enlargedBall = ball.transform;
        //enlargedBall.localScale = new Vector3(enlargedBall.localScale.x + ballSizeIncrease, enlargedBall.localScale.y + ballSizeIncrease, enlargedBall.localScale.z);
        originalSize = enlargedBall.localScale.x;
        lerpBigger = true;
        lerpStartTime = Time.time;
    }

    protected override void Revert()
    {
        switch (target)
        {
            case PowerupTarget.Ball:
                enlargedBall.localScale = new Vector3(enlargedBall.localScale.x - ballSizeIncrease, enlargedBall.localScale.y - ballSizeIncrease, enlargedBall.localScale.z);  // TODO add lerping
                break;

            case PowerupTarget.Player:
                enlargedPlayer.localScale = new Vector3(enlargedPlayer.localScale.x - playerSizeIncrease, enlargedPlayer.localScale.y, enlargedPlayer.localScale.z);  // TODO add lerping
                break;
        }
    }

    private void Update()
    {
        if (lerpBigger)
        {
            if (target == PowerupTarget.Player)
            {
                enlargedPlayer.localScale = new Vector3(originalSize + Mathf.Lerp(0, playerSizeIncrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), enlargedPlayer.localScale.y, enlargedPlayer.localScale.z);
            }
            else // Ball
            {
                enlargedBall.localScale = new Vector3(originalSize + Mathf.Lerp(0, ballSizeIncrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), originalSize + Mathf.Lerp(0, ballSizeIncrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), enlargedBall.localScale.z);
            }

            if ((Time.time - lerpStartTime) * (1 / sizeChangeDuration) > 1)
            {
                lerpBigger = false;
            }
        }
    }
}