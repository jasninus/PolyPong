using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : PowerupBase
{
    [SerializeField] private float ballSpeedIncrease, playerSpeedIncrease, enemySpeedDecrease;

    private Ball ball;

    private List<Player> enemies;
    private Player spedPlayer;

    protected override void BallActivate(GameObject ball)
    {
        this.ball = ball.GetComponent<Ball>();
        this.ball.ballSpeed += ballSpeedIncrease;
    }

    protected override void PlayerActivate(Player lastPlayerHit)
    {
        spedPlayer = lastPlayerHit;
        lastPlayerHit.playerSpeed += playerSpeedIncrease;
    }

    protected override void EnemyActivate(List<Player> enemies)
    {
        this.enemies = enemies;

        foreach (Player enemy in enemies)
        {
            enemy.playerSpeed -= enemySpeedDecrease;
        }
    }

    protected override void Revert()
    {
        switch (target)
        {
            case PowerupTarget.Ball:
                ball.ballSpeed -= ballSpeedIncrease;
                break;

            case PowerupTarget.Player:
                spedPlayer.playerSpeed -= playerSpeedIncrease;
                break;

            case PowerupTarget.Enemies:
                foreach (Player enemy in enemies)
                {
                    enemy.playerSpeed += enemySpeedDecrease;
                }
                break;
        }
    }
}