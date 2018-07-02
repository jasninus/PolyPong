using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Player
{
    public delegate void Spawn(GameObject ball);

    public static event Spawn BallSpawn;

    private int difficulty;

    private float[] difficultySpeedModifiers = { 1, 1.25f, 1.4f };

    private GameObject ball;

    private void Awake()
    {
        BallSpawn += GetNewBall;
    }

    private void GetNewBall(GameObject newBall)
    {
        ball = newBall;
    }

    private void FixedUpdate()
    {
        // If ball is further away on bots local axis than someValue
        {
            // If ball is to the left of bot on bots local axis
            // Move left
            // Else
            // Move right
        }
    }
}