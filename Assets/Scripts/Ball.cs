using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class Ball : MonoBehaviour
{
    [HideInInspector] public Player lastPlayerHit;

    public static float ballSpeedModifier;
    public float ballSpeed;
    [SerializeField] private float maxCurving, curvingPerHit, curveIntensity, curveRotationSpeed, sideHitRotation;
    private float curvingAmount;

    private AudioSource audio;

    private Rigidbody2D rb;

    private void Awake()
    {
        ballSpeed += ballSpeedModifier;
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
    }

    private void FixedUpdate() // TODO stop rotation if curvingAmount is 0 and test if is problem in circle
    {
        transform.Rotate(0, 0, curvingAmount * curveRotationSpeed);

        Vector2 curveVector = Quaternion.Euler(0, 0, 90) * rb.velocity * curvingAmount * curveIntensity;
        rb.velocity = (rb.velocity + curveVector).normalized * ballSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                PlayerCollision(other);
                break;

            case "Goal":
                GoalCollision(other);
                break;
        }
    }

    private void GoalCollision(Collision2D other)
    {
        Player hitPlayer = other.gameObject.GetComponent<Player>();

        if (!hitPlayer.hasShield)
        {
            lastPlayerHit = null;
            Destroy(gameObject);

            foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball")) // Destroy all other balls
            {
                Destroy(ball);
            }
        }

        InGameManager.playerToDestroy = hitPlayer.playerOrder;

        if (InGameManager.innerPoints.Count < 4)
        {
            if (InGameManager.innerPoints.Count == 3)
            {
                Points.AddPoints(hitPlayer.color);
            }

            hitPlayer.CircleDestroyPlayer();
            return;
        }

        hitPlayer.DestroyPlayer();
    }

    /// <summary>
    /// If the player is moving, curving gets increased otherwise is decreased
    /// </summary>
    /// <param name="hitPlayer">The hit player</param>
    private void CheckCurvingIncrease(Player hitPlayer)
    {
        if (hitPlayer.movingLeft)
        {
            curvingAmount -= curvingPerHit;
        }
        else if (hitPlayer.movingRight)
        {
            curvingAmount += curvingPerHit;
        }
        else if (curvingAmount > 0.1f || curvingAmount < -0.1f)
        {
            curvingAmount += curvingAmount > 0 ? -curvingPerHit : curvingPerHit;
        }
    }

    /// <summary>
    /// Ensures that rotation is never more than maxCurving
    /// </summary>
    private void ClampCurving()
    {
        if (curvingAmount > maxCurving)
        {
            curvingAmount = maxCurving;
        }
        else if (curvingAmount < -maxCurving)
        {
            curvingAmount = -maxCurving;
        }
    }

    private void PlayerCollision(Collision2D other)
    {
        audio.Play();

        lastPlayerHit = other.gameObject.transform.parent.GetComponent<Player>();
        CheckCurvingIncrease(lastPlayerHit);
        ClampCurving();

        ChangeExitAngle(CheckForSideCollision(other.transform));
    }

    /// <summary>
    /// Checks where on a player the ball is
    /// </summary>
    /// <param name="hitTransform">Transform of the player</param>
    /// <returns>Which part of the player the ball hit</returns>
    private Direction CheckForSideCollision(Transform hitTransform)
    {
        Vector2 fromTarget = transform.position - hitTransform.position;
        Vector2 projectedVector = Vector3.Project(fromTarget, hitTransform.right);

        Direction dir = projectedVector.magnitude > hitTransform.localScale.x / 4f ? hitTransform.InverseTransformDirection(projectedVector).x > 0 ? Direction.right : Direction.left : Direction.middle;

        return dir;
    }

    /// <summary>
    /// Exit angle of the ball can change based on where it hits a player
    /// </summary>
    /// <param name="hitPlace">Where the ball hit a player</param>
    private void ChangeExitAngle(Direction hitPlace)
    {
        if (hitPlace == Direction.middle)
        {
            return;
        }

        rb.velocity = (hitPlace == Direction.left ? Quaternion.Euler(0, 0, sideHitRotation) : Quaternion.Euler(0, 0, -sideHitRotation)) * rb.velocity;
    }
}