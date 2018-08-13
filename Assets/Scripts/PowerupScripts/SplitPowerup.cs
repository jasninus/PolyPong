using UnityEngine;

public class SplitPowerup : PowerupBase
{
    protected override void BallActivate(GameObject ball)
    {
        Rigidbody2D ballRB = ball.GetComponent<Rigidbody2D>();

        GameObject newBall = Instantiate(ball, ball.transform.position, Quaternion.identity);

        ballRB.velocity = Quaternion.Euler(0, 0, -50) * ballRB.velocity;
        newBall.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 50) * ballRB.velocity;
    }
}