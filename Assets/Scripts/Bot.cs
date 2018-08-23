using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Player
{
    [SerializeField] private int difficulty;

    private readonly float[] difficultySpeedModifiers = { 0.5f, 0.7f, 0.9f }, circleDifficultyModifiers = { 1f, 1.4f, 1.7f };
    [SerializeField] private const float deadpointRange = 0.05f;

    private GameObject ball;

    private void Awake()
    {
        GameStart.BallSpawn += GetNewBall;
    }

    private void Start()
    {
        difficulty = BotSelection.botDifficulties[color];

        playerSpeed *= difficultySpeedModifiers[difficulty > 0 ? difficulty - 1 : 0];
        circleSpeed *= circleDifficultyModifiers[difficulty > 0 ? difficulty - 1 : 0];
    }

    private void GetNewBall(GameObject newBall)
    {
        ball = newBall;
    }

    private void FixedUpdate()
    {
        if (!ball)
            return;

        if (LevelManager.isCircle)
        {
            CircleMovement();
        }
        else
        {
            LevelMovement();
        }
    }

    private void LevelMovement()
    {
        float minDis = CalculateMinDis();
        float xDiff = transform.InverseTransformVector(ball.transform.position - transform.GetChild(0).position).x;

        if (xDiff < -deadpointRange && CanMoveLeft(minDis))
        {
            MoveLeft();
        }
        else if (xDiff > deadpointRange && CanMoveRight(minDis))
        {
            MoveRight();
        }
        else
        {
            movingLeft = false;
            movingRight = false;
        }

        ClampMovement(minDis);
    }

    private void CircleMovement()
    {
        // TODO consider changing logic to be based on if left or right movement will bring bot closer to ball. This will probably make bots more difficult
        if (CalculateCircleXDiff() > 0 && transform.parent.rotation.eulerAngles.z < 270 - minRot)
        {
            CircleMoveRight();
        }
        else if (transform.parent.rotation.eulerAngles.z > 90 + minRot)
        {
            CircleMoveLeft();
        }
        else
        {
            movingLeft = false;
            movingRight = false;
        }
    }

    private float CalculateCircleXDiff()
    {
        Vector2 circleAxis = (LevelManager.innerLerpTo[0] - LevelManager.innerLerpTo[LevelManager.innerLerpTo.Count / 2]).normalized;
        float axisRotation = Mathf.Atan(circleAxis.y / circleAxis.x) * Mathf.Rad2Deg;

        Vector2 fromVector = ball.transform.position - transform.position;
        Vector2 rotatedVector = Quaternion.Euler(0, 0, axisRotation / 2 + (playerOrder == 1 ? 90 : 270)) * fromVector;

        return rotatedVector.x;
    }
}