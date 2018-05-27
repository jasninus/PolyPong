using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallStart : MonoBehaviour
{
    [SerializeField] public GameObject ballPrefab;
    private GameObject ball;

    [SerializeField] private float countDownTime, BallSpeed;
    public static float ballSpeed; // TODO ballSpeed should probably be moved to BallMovement

    private void Awake()
    {
        ballSpeed = BallSpeed;
    }

    /// <summary>
    /// Start the countdown to launch the ball
    /// </summary>
    /// <param name="levelCenter">Spawnpoint of the ball</param>
    public void StartCountdown(Vector2 levelCenter)
    {
        ball = Instantiate(ballPrefab, levelCenter, Quaternion.identity);
        StartCoroutine(ShootBall(ball, countDownTime));
    }

    private IEnumerator ShootBall(GameObject spawnedBall, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        float direction = Random.Range(0, 2 * Mathf.PI);
        spawnedBall.GetComponent<Rigidbody2D>().velocity = new Vector3(Mathf.Cos(direction), Mathf.Sin(direction)) * ballSpeed;
    }
}