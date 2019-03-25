using System;
using System.Linq;
using UnityEngine;

public class MenuLevel : LevelManager
{
    [SerializeField] private float playerMinVisibleDis, lerpSpeed;

    private readonly bool shouldLerpFromCircle;

    private const int MAX_PLAYERS = 6;
    private int activatedPlayerCount;

    private readonly PlayerColor previouslySelectedPlayer;

    private readonly DirectionArrows arrowManager;

    private readonly LevelSpawner spawner;

    [SerializeProperty("PlayerLerpStates"), SerializeField] private bool[] playerLerpStates = new bool[6];

    private bool[] PlayerLerpStates
    {
        get { return playerLerpStates; }
        set
        {
            Debug.Log("SETTING!");

            activatedPlayerCount = value.Count(s => s);

            if (activatedPlayerCount == 2)
            {
                StartLerpCircleSmaller(1); // TODO input correct thing. Maybe use value
            }

            playerLerpStates = value;
        }
    }

    private const float ONE_THIRD_PI = 1.0471975511965977461542144610932f;
    private readonly float[] playerWeights = new float[6];
    private float bottomClamp;

    protected override void Awake()
    {
        base.Awake();

        // Selection will always start on yellow and should therefore be true
        playerLerpStates[0] = true;
    }

    private void Start()
    {
        levelSpawner.SpawnLevel(6);
        meshManager.SetMaterials((PlayerColor[])Enum.GetValues(typeof(PlayerColor)));
    }

    public void LerpPlayerIn(PlayerColor color)
    {
        playerLerpStates[(int)color] = true;
    }

    public void LerpPlayerOut(PlayerColor color)
    {
        playerLerpStates[(int)color] = true;
    }

    protected void UpdateMenuLevel(float[] weightedRadians, Vector2 center)
    {
        innerPoints = pointManager.SpawnInnerPoints(innerPoints.Count, levelCenter, weightedRadians);
        LevelPoints.MovePoints(innerPoints, LevelPoints.GetWeightedCenter(innerPoints, bottomClamp, playerWeights), levelCenter);
        outerPoints = pointManager.GetOuterPoints(innerPoints, center);

        playerManager.UpdatePlayerPositions();
        playerManager.PlayersLookAtPoint(center);
        arqdutManager.UpdateArqdutPositions(innerPoints, levelCenter);

        DrawMesh(innerPoints.Count);
    }

    private void Update()
    {
        // Should not update if level should be circle
        if (activatedPlayerCount > 2)
        {
            UpdateMenuLevel(GetUpdatedWeights(), LevelPoints.GetWeightedCenter(innerPoints, bottomClamp, playerWeights));
            CheckPlayerVisibility();
        }
        else if (activatedPlayerCount == 2)
        {
            // TODO Lerp into circle
        }
        else
        {
            // TODO Player count must be one so level should be hidden
        }
    }

    private float[] GetUpdatedWeights()
    {
        bottomClamp = playerWeights.Aggregate((total, weight) => total + weight) / 6 - ONE_THIRD_PI;

        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            playerWeights[i] = Mathf.Clamp(playerWeights[i] + (playerLerpStates[i] ? 1 : -1) * lerpSpeed * Time.deltaTime, bottomClamp, 0);
        }

        return playerWeights;
    }

    private void CheckPlayerVisibility()
    {
        foreach (Player player in PlayerManager.players)
        {
            player.Visible = Vector2.Distance(player.points.left, player.points.right) > playerMinVisibleDis;
        }
    }
}

#region MaybeUsefulStuff

//private void SpawnLevel()
//{
//    arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.Color == ChooseControls.controls.First(c => c.Value.rightKey == KeyCode.None).Key));
//    arrowManager.SwitchArrowDirection();
//    arrowManager.FlipArrow();
//    StopAllCoroutines();
//    shouldLerpFromCircle = false;
//}

//private void Update()
//{
//    if (ChooseControls.playerStates.Count(p => p.Value != PlayerState.Deactivated) == 3)
//    {
//        SpawnLevel();
//    }
//}

//private void UpdateArqduts()
//{
//    arqdutManager.DestroyAllArqduts();
//    List<Vector2> arqdutSpawnPositions = new List<Vector2>(3) { innerPoints[0], innerPoints[innerPoints.Count - 2], innerPoints.Last() };
//    arqdutManager.SpawnArqduts(arqdutSpawnPositions, levelCenter);
//}

//private void UpdateLerpLists(int index, List<PlayerColor> activatedColors)
//{
//    innerPoints.Insert(index, index == innerPoints.Count ? innerPoints[0] : innerPoints[index]);
//    outerPoints = pointManager.GetOuterPoints(innerPoints);

//    innerLerpTo = pointManager.SpawnInnerPoints(activatedColors.Count, levelCenter);
//    float rotateAmount = previousRotation + addPlayerRotationConstants[activatedColors.Count - 1][index];
//    previousRotation = rotateAmount;
//    pointManager.RotatePoints(innerLerpTo, rotateAmount);
//    outerLerpTo = pointManager.GetOuterPoints(innerLerpTo);
//}

//private void UpdateManagers(int index, PlayerColor color)
//{
//    arqdutManager.DestroyAllArqduts();
//    arqdutManager.SpawnArqduts(innerPoints, levelCenter);
//    meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(innerPoints, outerPoints));
//    meshManager.SetMaterials();
//    playerManager.DestroyAllPlayers();
//    playerManager.SpawnPlayers(pointManager.radius, ChooseControls.playerStates.Where(o => o.Value != PlayerState.Deactivated).Select(i => i.Key).ToArray());
//    arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.Color == color));
//    HidePlayer(color);
//    StartCoroutine(RevealPlayer(PlayerManager.players.First(p => p.Color == color)));

//    playerManager.PlayersLookAtPoint(levelCenter);
//    meshManager.AddIndicesAndDrawMesh(innerPoints.Count);
//}

//private void HidePlayer(PlayerColor color)
//{
//    Player player = PlayerManager.players.First(p => p.Color == color);
//    player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

//    if (player.transform.GetChild(0).childCount > 0)
//        player.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
//}

//private IEnumerator RevealPlayer(Player player)
//{
//    yield return new WaitUntil(() => Vector2.Distance(player.points.left, player.points.right) > disBeforePlayerSpawn);

//    player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
//    player.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
//}

//private void UpdateCircleSpawningPlayerPosition()
//{
//    circleSpawningPlayer.points = new Player.LeftRightPoints
//    {
//        left = innerPoints[innerPoints.Count - 2],
//        right = innerPoints[innerPoints.Count - 1]
//    };

//    circleSpawningPlayer.transform.position = (circleSpawningPlayer.points.left + circleSpawningPlayer.points.right) * 0.5f;
//}

//private void StartFromCircleLerp(int index, List<PlayerColor> activatedColor)
//{
//    UpdateArqduts();
//    shouldLerpFromCircle = true;
//    lerpedAmount = 1;

//    PlayerColor[] colors = ChooseControls.playerStates.Where(o => o.Value).Select(i => i.Key).ToArray();
//    BotSelection.botDifficulties[colors.Last()] = 0;
//    Player p = playerManager.SpawnPlayer(pointManager.radius, colors, 0);

//    p.playerOrder = index;
//    p.color = activatedColor[index];
//    p.transform.GetChild(0).GetComponent<SpriteRenderer>().color = MeshManager.materials[p.color].color;

//    p.points = new Player.LeftRightPoints
//    {
//        left = innerPoints[innerPoints.Count - 2],
//        right = innerPoints[innerPoints.Count - 1]
//    };

//    circleSpawningPlayer = p;
//    arrowManager.AttachLeftArrow(p);
//    HidePlayer(p.color);
//    StartCoroutine(RevealPlayer(p));

//    playerManager.SetFromCircleIndexes(index);

//    meshManager.SetMaterials();
//}

#endregion MaybeUsefulStuff