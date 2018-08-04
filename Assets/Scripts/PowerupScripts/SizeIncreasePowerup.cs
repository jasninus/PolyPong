using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeIncreasePowerup : PowerupBase
{
    [SerializeField] private float playerSizeIncrease, ballSizeIncrease, sizeChangeDuration;
    private float lerpStartTime, originalSize;

    private bool lerpBigger, lerpSmaller;

    private Transform enlargedPlayerTransform, enlargedBall;

    private Player enlargedPlayer;

    protected override void PlayerActivate(Player lastPlayerHit)
    {
        enlargedPlayerTransform = lastPlayerHit.transform.GetChild(0);
        originalSize = enlargedPlayerTransform.localScale.x;
        lerpBigger = true;
        lerpStartTime = Time.time;
        enlargedPlayer = lastPlayerHit;

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
                if (!enlargedBall)
                    return;

                originalSize = enlargedBall.localScale.x;
                lerpSmaller = true;
                lerpStartTime = Time.time;
                break;

            case PowerupTarget.Player:
                if (!enlargedPlayerTransform)
                    return;

                originalSize = enlargedPlayerTransform.localScale.x;
                lerpSmaller = true;
                lerpStartTime = Time.time;
                break;
        }
    }

    private void Update()
    {
        if (lerpBigger)
        {
            if (target == PowerupTarget.Player && enlargedPlayerTransform)
            {
                enlargedPlayerTransform.localScale = new Vector3(originalSize + Mathf.Lerp(0, playerSizeIncrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), enlargedPlayerTransform.localScale.y, enlargedPlayerTransform.localScale.z);
                enlargedPlayer.ClampMovement(enlargedPlayer.CalculateMinDis());
            }
            else if (enlargedBall) // Ball
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
            if (target == PowerupTarget.Player && enlargedPlayerTransform)
            {
                enlargedPlayerTransform.localScale = new Vector3(originalSize - Mathf.Lerp(0, playerSizeIncrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), enlargedPlayerTransform.localScale.y, enlargedPlayerTransform.localScale.z);
            }
            else if (enlargedBall) // Ball
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