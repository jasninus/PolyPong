using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeIncreasePowerup : PowerupBase
{
    [SerializeField] private float playerSizeIncrease, ballSizeIncrease;

    private Transform enlargedPlayer, enlargedBall;

    protected override void PlayerActivate(Player lastPlayerHit)
    {
        enlargedPlayer = lastPlayerHit.transform.GetChild(0);
        enlargedPlayer.localScale = new Vector3(enlargedPlayer.localScale.x + playerSizeIncrease, enlargedPlayer.localScale.y, enlargedPlayer.localScale.z); // TODO add lerping
    }

    protected override void BallActivate(GameObject ball)
    {
        enlargedBall = ball.transform;
        enlargedBall.localScale = new Vector3(enlargedBall.localScale.x + ballSizeIncrease, enlargedBall.localScale.y + ballSizeIncrease, enlargedBall.localScale.z);  // TODO add lerping
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
}