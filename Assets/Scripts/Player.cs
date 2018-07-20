using UnityEngine;

public class Player : MonoBehaviour
{
    public float playerSpeed, circleSpeed, minDis, minRot;

    public bool hasShield;

    public struct LeftRightPoints
    {
        public Vector2 left, right;
    }

    public LeftRightPoints points;

    private Vector3 childStart, childLerpFrom;

    private LevelManager levelManager;

    public PlayerColors color;

    public int playerOrder;

    /// <summary>
    /// Call this function on creation of player instead of a constructor
    /// </summary>
    public void Initialize(PlayerColors color, Vector2 leftPoint, Vector2 rightPoint, int playerOrder, LevelManager levelManager, float playerSpeed, float circleSpeed, float radius)
    {
        LevelManager.OnPlayerDestroy += CheckPlayerOrderDecrease;
        this.color = color;
        points = new LeftRightPoints { left = leftPoint, right = rightPoint };
        this.playerOrder = playerOrder;
        this.levelManager = levelManager;
        this.playerSpeed = playerSpeed;
        this.circleSpeed = circleSpeed;

        childStart = transform.GetChild(0).localPosition;
        CalculateMinRot(radius);
    }

    /// <summary>
    /// Calculate the rotation clamp values for circle movement
    /// </summary>
    /// <param name="radius">Radius of the level</param>
    private void CalculateMinRot(float radius)
    {
        minRot = 3.85f * Mathf.Acos(Mathf.Deg2Rad * ((Mathf.Pow(radius, 2) * 2 - Mathf.Pow(transform.GetChild(0).localScale.x * transform.localScale.x / 2, 2)) / (2 * Mathf.Pow(radius, 2))));
    }

    private void DeactivePlayer()
    {
        transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void DestroyPlayer()
    {
        if (hasShield)
        {
            hasShield = false;
            return;
        }

        ChooseControls.activatedPlayers[color] = false;
        levelManager.StartLerpLevelSmaller(playerOrder);
        LevelManager.OnPlayerDestroy -= CheckPlayerOrderDecrease;
        DeactivePlayer();
    }

    public void CircleDestroyPlayer()
    {
        if (hasShield)
        {
            hasShield = false;
            return;
        }

        transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        levelManager.StartLerpCircleSmaller(playerOrder);
        DeactivePlayer();
    }

    private void CheckPlayerOrderDecrease(int destroyedPlayerOrder)
    {
        if (playerOrder > destroyedPlayerOrder)
        {
            playerOrder--;
        }
    }

    private void FixedUpdate()
    {
        if (!LevelManager.isCircle && !LevelManager.shouldLerpToCircle)
        {
            LevelMovement();
        }
        else if (LevelManager.isCircle && transform.parent != null)
        {
            CircleMovement();
        }
    }

    public void SetChildLerpFrom()
    {
        childLerpFrom = transform.GetChild(0).localPosition;
    }

    public void LerpPlayerPosition(float lerpAmount)
    {
        transform.GetChild(0).localPosition = Vector3.Lerp(childLerpFrom, childStart, lerpAmount * 2);
    }

    private void LevelMovement()
    {
        minDis = 0.38f / Mathf.Tan(((((LevelManager.innerPoints.Count - 2) * 180f) / LevelManager.innerPoints.Count) / 2f) * Mathf.Deg2Rad); // TODO this should only be calculated on start and whilst lerping level
        // Left key control
        if (Input.GetKey(ChooseControls.controls[color].leftKey) && Vector3.Project(transform.GetChild(0).position - (Vector3)points.left, (transform.position - (Vector3)points.left).normalized).magnitude - transform.GetChild(0).localScale.x * transform.localScale.x / 2 > minDis)
        {
            MoveLeft();
        }
        // Right key control
        if (Input.GetKey(ChooseControls.controls[color].rightKey) && Vector3.Project(transform.GetChild(0).position - (Vector3)points.right, (transform.position - (Vector3)points.right).normalized).magnitude - transform.GetChild(0).localScale.x * transform.localScale.x / 2 > minDis)
        {
            MoveRight();
        }

        transform.GetChild(0).transform.localPosition = new Vector3(transform.GetChild(0).transform.localPosition.x, 10, transform.GetChild(0).transform.localPosition.z);
    } // TODO clamp player movement every frame

    private void MoveLeft()
    {
        transform.GetChild(0).position += ((Vector3)points.left - transform.position).normalized * playerSpeed;
    }

    private void MoveRight()
    {
        transform.GetChild(0).position += ((Vector3)points.right - transform.position).normalized * playerSpeed;
    }

    private void CircleMovement()
    {
        if (Input.GetKey(ChooseControls.controls[color].leftKey) && transform.parent.rotation.eulerAngles.z > 90 + minRot)
        {
            transform.parent.Rotate(0, 0, -circleSpeed);
        }

        if (Input.GetKey(ChooseControls.controls[color].rightKey) && transform.parent.rotation.eulerAngles.z < 270 - minRot)
        {
            transform.parent.Rotate(0, 0, circleSpeed);
        }
    }
}