using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public delegate void Spawn(GameObject ball);

    public static event Spawn BallSpawn;

    public GameObject ball, directionArrow;
    private readonly List<GameObject> spawnedDirArrows = new List<GameObject>();
    private GameObject _ball;

    private readonly List<Vector2> normalizedBallDirections = new List<Vector2>();

    [SerializeField] private Text countdownText;

    [SerializeField] private float countDownTime, dirArrowDisFromCenter;

    [SerializeField] private int countDownAmount;
    private int currentCountDown;

    /// <summary>
    /// Start the countdown to launch the ball
    /// </summary>
    /// <param name="levelCenter">Spawnpoint of the ball</param>
    public void StartCountdown(Vector2 levelCenter)
    {
        if (!ChooseControls.gameStarted)
            return;

        // There cannot be two simultaneous countdowns
        if (currentCountDown == 0)
        {
            StartCoroutine(CountDown(levelCenter));
        }
    }

    public void ResetCountdown()
    {
        currentCountDown = 0;
        StopAllCoroutines();

        foreach (GameObject arrow in spawnedDirArrows)
        {
            Destroy(arrow);
        }

        spawnedDirArrows.Clear();
        normalizedBallDirections.Clear();

        countdownText.text = "";
        Time.timeScale = 1;
    }

    public IEnumerator CountDown(Vector2 ballPositions)
    {
        if (currentCountDown == 0)
        {
            float ballDirection = Random.Range(0, 2 * Mathf.PI);
            normalizedBallDirections.Add(new Vector2(Mathf.Cos(ballDirection), Mathf.Sin(ballDirection)));
            spawnedDirArrows.Add(SpawnDirectionArrow(normalizedBallDirections[0], ballDirection * Mathf.Rad2Deg));
        }

        countdownText.text = (countDownAmount - currentCountDown).ToString();
        currentCountDown++;

        yield return new WaitForSeconds(countDownTime);

        if (countDownAmount > currentCountDown)
        {
            StartCoroutine(CountDown(ballPositions));
        }
        else
        {
            StartRound(ballPositions);
        }
    }

    public IEnumerator CountDown(Vector2[] ballPositions, Vector2[] directions)
    {
        if (currentCountDown == 0)
        {
            for (int i = 0; i < ballPositions.Length; i++)
            {
                float ballDirection = directions[i].y >= 0 ? Mathf.Acos(directions[i].normalized.x) : 2 * Mathf.PI - Mathf.Acos(directions[i].normalized.x);
                normalizedBallDirections.Add(directions[i].normalized);
                spawnedDirArrows.Add(SpawnDirectionArrow(normalizedBallDirections[i], ballDirection * Mathf.Rad2Deg));
                spawnedDirArrows[i].transform.position = ballPositions[i];
            }
        }

        countdownText.text = (countDownAmount - currentCountDown).ToString();
        currentCountDown++;

        yield return new WaitForSecondsRealtime(countDownTime);

        if (countDownAmount > currentCountDown)
        {
            StartCoroutine(CountDown(ballPositions, directions));
        }
        else
        {
            Time.timeScale = 1;
            StartRound(ballPositions);
        }
    }

    private void StartRound(Vector2 ballStartPosition)
    {
        currentCountDown = 0;
        countdownText.text = "";

        SpawnBall(ballStartPosition, normalizedBallDirections[0]);
        Destroy(spawnedDirArrows[0]);

        spawnedDirArrows.Clear();
        normalizedBallDirections.Clear();
    }

    private void StartRound(Vector2[] ballStartPositions)
    {
        currentCountDown = 0;
        countdownText.text = "";

        for (int i = 0; i < ballStartPositions.Length; i++)
        {
            SpawnBall(ballStartPositions[i], normalizedBallDirections[i]);
            Destroy(spawnedDirArrows[i]);
        }

        spawnedDirArrows.Clear();
        normalizedBallDirections.Clear();
    }

    private GameObject SpawnDirectionArrow(Vector2 normSpawnPos, float startAngle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, startAngle + 180);
        return Instantiate(directionArrow, normSpawnPos * dirArrowDisFromCenter, rotation);
    }

    private void SpawnBall(Vector2 levelCenter, Vector2 normalizedDirection)
    {
        _ball = Instantiate(ball, levelCenter, Quaternion.identity);
        _ball.GetComponent<Rigidbody2D>().velocity = normalizedDirection * ball.GetComponent<Ball>().ballSpeed;

        BallSpawn?.Invoke(_ball);
    }
}