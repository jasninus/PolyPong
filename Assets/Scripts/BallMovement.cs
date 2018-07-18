using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public Player lastPlayerHit;

    [SerializeField] private float maxCurving, curvingPerHit, curveIntensity, curveRotationSpeed, sideHitRotation;
    private float curvingAmount;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, curvingAmount * curveRotationSpeed);

        Vector2 curveVector = Quaternion.Euler(0, 0, 90) * rb.velocity * curvingAmount * curveIntensity;
        rb.velocity = (rb.velocity + curveVector).normalized * GameStart.ballSpeed;
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

            case "CircleCollider":
                Destroy(gameObject);
                break;
        }
    }

    private void GoalCollision(Collision2D other)
    {
        if (!other.gameObject.GetComponent<Player>().hasShield)
        {
            lastPlayerHit = null;
            Destroy(gameObject);
        }

        LevelManager.playerToDestroy = other.gameObject.GetComponent<Player>().playerOrder;

        if (LevelManager.innerPoints.Count < 4)
        {
            other.gameObject.GetComponent<Player>().CircleDestroyPlayer();
            return;
        }

        other.gameObject.GetComponent<Player>().DestroyPlayer();
    }

    /// <summary>
    /// If the player is moving, curving gets increased otherwise is decreased
    /// </summary>
    /// <param name="hitPlayer">The hit player</param>
    private void CheckCurvingIncrease(Player hitPlayer)
    {
        if (Input.GetKey(ChooseControls.controls[hitPlayer.color].leftKey))
        {
            curvingAmount -= curvingPerHit;
        }
        else if (Input.GetKey(ChooseControls.controls[hitPlayer.color].rightKey))
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