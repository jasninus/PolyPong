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
        originalSize = enlargedPlayer.localScale.x;
        lerpBigger = true;
        lerpStartTime = Time.time;

        Invoke("RevertToNormalSize", duration - sizeChangeDuration);
    }

    protected override void BallActivate(GameObject ball)
    {
        enlargedBall = ball.transform;
        originalSize = enlargedBall.localScale.x;
        lerpBigger = true;
        lerpStartTime = Time.time;

        Invoke("RevertToNormalSize", duration - sizeChangeDuration);
    }

    private void RevertToNormalSize()
    {
        switch (target)
        {
            case PowerupTarget.Ball:
                originalSize = enlargedBall.localScale.x;
                lerpSmaller = true;
                lerpStartTime = Time.time;
                break;

            case PowerupTarget.Player:
                originalSize = enlargedPlayer.localScale.x;
                lerpSmaller = true;
                lerpStartTime = Time.time;
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
        else if (lerpSmaller)
        {
            if (target == PowerupTarget.Player)
            {
                enlargedPlayer.localScale = new Vector3(originalSize - Mathf.Lerp(0, playerSizeIncrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), enlargedPlayer.localScale.y, enlargedPlayer.localScale.z);
            }
            else // Ball
            {
                enlargedBall.localScale = new Vector3(originalSize - Mathf.Lerp(0, ballSizeIncrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), originalSize - Mathf.Lerp(0, ballSizeIncrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), enlargedBall.localScale.z);
            }

            if ((Time.time - lerpStartTime) * (1 / sizeChangeDuration) > 1)
            {
                lerpBigger = false;
            }
        }
    }
}