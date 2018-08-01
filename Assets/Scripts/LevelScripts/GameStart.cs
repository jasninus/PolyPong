using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public delegate void Spawn(GameObject ball);

    public static event Spawn BallSpawn;

    [SerializeField] public GameObject ball, directionArrow;
    private GameObject _ball, spawnedDirArrow;

    private Vector2 normalizedBallDirection;

    [SerializeField] private Text countdownText;

    [SerializeField] private float countDownTime, dirArrowDisFromCenter;
    private float ballDirection;

    [SerializeField] private int countDownAmount;
    private int currentCountDown;

    /// <summary>
    /// Start the countdown to launch the ball
    /// </summary>
    /// <param name="levelCenter">Spawnpoint of the ball</param>
    public void StartCountdown(Vector2 levelCenter)
    {
        // There cannot be two simultaneous countdowns
        if (currentCountDown == 0)
        {
            StartCoroutine(CountDown(levelCenter));
        }
    }

    private IEnumerator CountDown(Vector2 levelCenter)
    {
        if (currentCountDown == 0)
        {
            ballDirection = Random.Range(0, 2 * Mathf.PI);
            normalizedBallDirection = new Vector2(Mathf.Cos(ballDirection), Mathf.Sin(ballDirection));
            spawnedDirArrow = SpawnDirectionArrow(normalizedBallDirection, ballDirection * Mathf.Rad2Deg);
        }

        countdownText.text = (countDownAmount - currentCountDown).ToString();
        currentCountDown++;

        yield return new WaitForSeconds(countDownTime);

        if (countDownAmount > currentCountDown)
        {
            StartCoroutine(CountDown(levelCenter));
        }
        else
        {
            StartRound(levelCenter);
        }
    }

    private void StartRound(Vector2 levelCenter)
    {
        currentCountDown = 0;
        SpawnBall(levelCenter);
        Destroy(spawnedDirArrow);
        countdownText.text = "";
    }

    private GameObject SpawnDirectionArrow(Vector2 normSpawnPos, float startAngle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, startAngle + 180);
        return Instantiate(directionArrow, normSpawnPos * dirArrowDisFromCenter, rotation);
    }

    private void SpawnBall(Vector2 levelCenter)
    {
        _ball = Instantiate(ball, levelCenter, Quaternion.identity);
        _ball.GetComponent<Rigidbody2D>().velocity = normalizedBallDirection * ball.GetComponent<BallMovement>().ballSpeed;

        BallSpawn?.Invoke(_ball);
    }
}