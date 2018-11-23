using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SizeDecreasePowerup : PowerupBase
{
    [SerializeField] private float playerSizeDecrease, ballSizeDecrease, sizeChangeDuration;
    private float lerpStartTime, originalBallSize;
    private float[] originalSizes;

    private bool lerpSmaller, lerpBigger;

    private Transform[] reducedEnemyTransforms;
    private Transform reducedBall;

    private List<Player> reducedEnemies;

    protected override void EnemyActivate(List<Player> enemies)
    {
        reducedEnemyTransforms = enemies.Select(e => e.transform.GetChild(0)).ToArray();
        originalSizes = reducedEnemyTransforms.Select(e => e.localScale.x).ToArray();
        lerpSmaller = true;
        lerpStartTime = Time.time;
        reducedEnemies = enemies;

        Invoke("RevertToNormalSize", duration - sizeChangeDuration);
    }

    protected override void BallActivate(GameObject ball)
    {
        reducedBall = ball.transform;
        originalBallSize = reducedBall.localScale.x;
        lerpSmaller = true;
        lerpStartTime = Time.time;

        Invoke("RevertToNormalSize", duration - sizeChangeDuration);
    }

    private void RevertToNormalSize()
    {
        switch (target)
        {
            case PowerupTarget.Ball:
                if (!reducedBall)
                    return;

                originalBallSize = reducedBall.localScale.x;
                lerpBigger = true;
                lerpStartTime = Time.time;
                break;

            case PowerupTarget.Enemies:
                if (!reducedEnemyTransforms[0])
                    return;

                originalSizes = reducedEnemyTransforms.Select(e => e.localScale.x).ToArray();
                lerpBigger = true;
                lerpStartTime = Time.time;
                break;
        }
    }

    private void Update()
    {
        if (lerpSmaller)
        {
            LerpSmaller();
        }
        else if (lerpBigger)
        {
            LerpBigger();
        }
    }

    private void LerpBigger()
    {
        if (target == PowerupTarget.Enemies && reducedEnemyTransforms[0])
        {
            for (int i = 0; i < reducedEnemyTransforms.Length; i++)
            {
                reducedEnemyTransforms[i].localScale = new Vector3(
                    originalSizes[i] + Mathf.Lerp(0, playerSizeDecrease,
                        (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), reducedEnemyTransforms[i].localScale.y,
                    reducedEnemyTransforms[i].localScale.z);

                reducedEnemies[i].ClampMovement(reducedEnemies[i].CalculateMinDis());
            }
        }
        else if (reducedBall) // Ball
        {
            reducedBall.localScale = new Vector3(
                originalBallSize + Mathf.Lerp(0, ballSizeDecrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)),
                originalBallSize + Mathf.Lerp(0, ballSizeDecrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)),
                reducedBall.localScale.z);
        }

        if ((Time.time - lerpStartTime) * (1 / sizeChangeDuration) > 1)
        {
            lerpSmaller = false;
        }
    }

    private void LerpSmaller()
    {
        if (target == PowerupTarget.Enemies && reducedEnemyTransforms[0])
        {
            for (int i = 0; i < reducedEnemyTransforms.Length; i++)
            {
                reducedEnemyTransforms[i].localScale = new Vector3(
                    originalSizes[i] - Mathf.Lerp(0, playerSizeDecrease,
                        (Time.time - lerpStartTime) * (1 / sizeChangeDuration)), reducedEnemyTransforms[i].localScale.y,
                    reducedEnemyTransforms[i].localScale.z);

                reducedEnemies[i].ClampMovement(reducedEnemies[i].CalculateMinDis());
            }
        }
        else if (reducedBall) // Ball
        {
            reducedBall.localScale = new Vector3(
                originalBallSize - Mathf.Lerp(0, ballSizeDecrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)),
                originalBallSize - Mathf.Lerp(0, ballSizeDecrease, (Time.time - lerpStartTime) * (1 / sizeChangeDuration)),
                reducedBall.localScale.z
            );
        }

        if ((Time.time - lerpStartTime) * (1 / sizeChangeDuration) > 1)
        {
            lerpSmaller = false;
        }
    }
}