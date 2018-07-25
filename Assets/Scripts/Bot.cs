using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Player
{

    [SerializeField] private int difficulty;

    private readonly float[] difficultySpeedModifiers = { 0.5f, 0.7f, 0.9f };

    private GameObject ball;

    private void Awake()
    {
        GameStart.BallSpawn += GetNewBall;
        
    }

    private void Start()
    {
        difficulty = BotSelection.botDifficulties[color];

        playerSpeed *= difficultySpeedModifiers[difficulty - 1];
    }

    private void GetNewBall(GameObject newBall)
    {
        ball = newBall;
    }

    private void FixedUpdate()
    {
        if (!ball)
            return;

        float minDis = GetMinDis();

        if (transform.InverseTransformVector(ball.transform.position - transform.GetChild(0).position).x < 0 && CanMoveLeft(minDis))
        {
            MoveLeft();
        }
        else if(CanMoveRight(minDis)) // TODO there should probably be added a dead point in the middle, so there isn't constant jittering
        {
            MoveRight();
        }
        else
        {
            // No movement
        }
   
        //Debug.Log(color + " " + transform.InverseTransformVector(ball.transform.position - transform.GetChild(0).position).x);

        // Vector3.Project(ball.transform.position - transform.GetChild(0).position, (Vector3) points.left - transform.GetChild(0).position).magnitude > 0
        //Debug.Log(color + " " + Vector3.Project(ball.transform.position - transform.GetChild(0).position, (Vector3)points.left - transform.GetChild(0).position));

        // If ball is further away on bots local axis than someValue
        {
            // If ball is to the left of bot on bots local axis && CanMoveLeft(minDis)
            // Move left
            // Else if (CanMoveRight(minDis))
            // Move right


        }
    }

    private void OnDrawGizmos()
    {
    }
}