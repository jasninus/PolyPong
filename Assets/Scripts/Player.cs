using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void PlayerDeath();

    public static event PlayerDeath UpdateScoreboard;

    public float playerSpeed, circleSpeed, minDis, minRot;

    public bool hasShield, movingLeft, movingRight;

    public struct LeftRightPoints
    {
        public Vector2 left, right;
    }

    public LeftRightPoints points;

    private Vector3 childStartPos, childLerpFrom;

    private InGameManager _inGameManager;

    public PlayerColor Color;

    public int playerOrder;

    /// <summary>
    /// Call this function on creation of player instead of a constructor
    /// </summary>
    public void Initialize(PlayerColor color, Vector2 leftPoint, Vector2 rightPoint, int playerOrder, InGameManager inGameManager, float playerSpeed, float circleSpeed, float radius)
    {
        InGameManager.OnPlayerDestroy += CheckPlayerOrderDecrease;
        this.Color = color;
        points = new LeftRightPoints { left = leftPoint, right = rightPoint };
        this.playerOrder = playerOrder;
        this._inGameManager = inGameManager;
        this.playerSpeed = playerSpeed;
        this.circleSpeed = circleSpeed;

        childStartPos = transform.GetChild(0).localPosition;
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

        if (ChooseControls.gameStarted)
            Points.AddPoints(Color);

        ChooseControls.playerStates[Color] = PlayerState.Deactivated;
        _inGameManager.StartLerpLevelSmaller(playerOrder);
        InGameManager.OnPlayerDestroy -= CheckPlayerOrderDecrease;
        DeactivePlayer();

        UpdateScoreboard?.Invoke();
    }

    public void CircleDestroyPlayer()
    {
        if (hasShield)
        {
            hasShield = false;
            return;
        }

        transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        _inGameManager.StartLerpCircleSmaller(playerOrder);
        DeactivePlayer();
        this.enabled = false;

        UpdateScoreboard?.Invoke();
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
        if (!ChooseControls.gameStarted)
            return;

        if (!InGameManager.isCircle && !InGameManager.shouldLerpToCircle)
        {
            LevelMovement();
        }
        else if (InGameManager.isCircle && transform.parent != null)
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
        transform.GetChild(0).localPosition = Vector3.Lerp(childLerpFrom, childStartPos, lerpAmount * 2);
    }

    #region movement

    private void LevelMovement()
    {
        minDis = CalculateMinDis();

        // Left key control
        if (Input.GetKey(ChooseControls.controls[Color].leftKey) && CanMoveLeft(minDis))
        {
            MoveLeft();
        }
        // Right key control
        else if (Input.GetKey(ChooseControls.controls[Color].rightKey) && CanMoveRight(minDis))
        {
            MoveRight();
        }
        else
        {
            movingLeft = false;
            movingRight = false;
        }

        transform.GetChild(0).transform.localPosition = new Vector3(transform.GetChild(0).transform.localPosition.x, 10, transform.GetChild(0).transform.localPosition.z);

        // TODO clamp player movement every frame
    }

    public void ClampMovement(float minDis)
    {
        if (!CanMoveLeft(minDis))
        {
            MoveRight();
        }

        if (!CanMoveRight(minDis))
        {
            MoveLeft();
        }
    }

    public float CalculateMinDis()
    {
        return 0.38f / Mathf.Tan(((((LevelManager.innerPoints.Count - 2) * 180f) / LevelManager.innerPoints.Count) / 2f) * Mathf.Deg2Rad);
    }

    protected bool CanMoveLeft(float minDis)
    {
        return Vector3.Project(transform.GetChild(0).position - (Vector3)points.left, (transform.position - (Vector3)points.left).normalized).magnitude - transform.GetChild(0).localScale.x * transform.localScale.x / 2 > minDis;
    }

    protected bool CanMoveRight(float minDis)
    {
        return Vector3.Project(transform.GetChild(0).position - (Vector3)points.right, (transform.position - (Vector3)points.right).normalized).magnitude - transform.GetChild(0).localScale.x * transform.localScale.x / 2 > minDis;
    }

    protected void MoveLeft()
    {
        transform.GetChild(0).position += ((Vector3)points.left - transform.position).normalized * playerSpeed;
        movingLeft = true;
        movingRight = false;
    }

    protected void MoveRight()
    {
        transform.GetChild(0).position += ((Vector3)points.right - transform.position).normalized * playerSpeed;
        movingRight = true;
        movingLeft = false;
    }

    private void CircleMovement()
    {
        // TODO convert to protected functions, so they can be called from Bot. Hey, good-lookin' ;)

        if (Input.GetKey(ChooseControls.controls[Color].leftKey) && transform.parent.rotation.eulerAngles.z > 90 + minRot)
        {
            CircleMoveLeft();
        }

        if (Input.GetKey(ChooseControls.controls[Color].rightKey) && transform.parent.rotation.eulerAngles.z < 270 - minRot)
        {
            CircleMoveRight();
        }
        else
        {
            movingLeft = false;
            movingRight = false;
        }
    }

    protected void CircleMoveRight()
    {
        transform.parent.Rotate(0, 0, circleSpeed);
        movingRight = true;
        movingLeft = false;
    }

    protected void CircleMoveLeft()
    {
        transform.parent.Rotate(0, 0, -circleSpeed);
        movingLeft = true;
        movingRight = false;
    }

    #endregion movement
}