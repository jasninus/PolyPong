using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

// TODO This whole system should probably be hooked up to the psychadelic size system instead. When a player is added, it's weight should start lerping from 0 to 1. When a player is removed, it's weight should lerp from 1 to 0. All players should be able to lerp at the same time. All players' weight will be normalized to calculate their actual weight
public class MenuInGameManager : InGameManager
{
    public static bool levelIsSpawned;
    private bool queueIsRunning, executeLevelLerpRunning;

    private PlayerColors previouslySelectedPlayer;

    private readonly int[][] addPlayerRotationConstants =
    {
        new[] { 0 },
        new[] { 0 },
        new[] { 0 },
        new[] { -45, -15, 15, 45 },
        new[] { -36, -18, 0, 18, 36 },
        new[] { -30, -18, -6, 6, 18, 30 }
    };

    [SerializeField] private float disBeforePlayerSpawn;

    private readonly List<QueueItem> queue = new List<QueueItem>();

    private struct QueueItem
    {
        // False for smaller, true for bigger
        public bool lerpType;

        public PlayerColors color;
    }

    protected override void Awake()
    {
        base.Awake();

        ChooseControls.PlayerAmountIncreased += QueueAddPlayer;
        ChooseControls.PlayerAmountDecreased += QueueRemovePlayer;
        ChooseControls.ForceAddPlayer += AddPlayer;
    }

    protected override void Start()
    {
        // REASON Here to make sure level is not spawned immediately
    }

    private void SpawnLevel()
    {
        levelSpawner.SpawnCircle();
        arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.color == ChooseControls.controls.First(c => c.Value.rightKey == KeyCode.None).Key));
        arrowManager.SwitchArrowDirection();
        arrowManager.FlipArrow();
        StopAllCoroutines();
        shouldLerpFromCircle = false;
        levelIsSpawned = true;
    }

    private void Update()
    {
        if (ChooseControls.activatedPlayers.Count(p => p.Value) == 2 && !levelIsSpawned)
        {
            SpawnLevel();
        }

        if (queue.Count > 0 && !queueIsRunning) // Start queue if queue is not running and there are queued items
        {
            StartCoroutine(Queue());
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (shouldLerpFromCircle)
        {
            UpdateCircleSpawningPlayerPosition();
        }
    }

    private void UpdateCircleSpawningPlayerPosition()
    {
        circleSpawningPlayer.points = new Player.LeftRightPoints
        {
            left = innerPoints[innerPoints.Count - 2],
            right = innerPoints[innerPoints.Count - 1]
        };

        circleSpawningPlayer.transform.position = (circleSpawningPlayer.points.left + circleSpawningPlayer.points.right) * 0.5f;
    }

    private void QueueAddPlayer(PlayerColors color)
    {
        queue.Add(new QueueItem { lerpType = true, color = color });
    }

    private void QueueRemovePlayer(PlayerColors color)
    {
        queue.Add(new QueueItem { lerpType = false, color = color });
    }

    private void UpdateActivatedPlayers(PlayerColors selectedColor)
    {
        if (ChooseControls.controls[previouslySelectedPlayer].leftKey == KeyCode.None && BotSelection.botDifficulties[previouslySelectedPlayer] == 0)
        {
            ChooseControls.activatedPlayers[previouslySelectedPlayer] = false;
            RemovePlayer(previouslySelectedPlayer);
        }
    }

    private void StartAddPlayer(PlayerColors color)
    {
        UpdateActivatedPlayers(color);

        if (!ChooseControls.activatedPlayers[color] && ChooseControls.activatedPlayers.Count(p => p.Value) > 1)
        {
            AddPlayer(color);
        }
    }

    private void AddPlayer(PlayerColors color)
    {
        // Find out what innerPoint should be duplicated to accomodate new player
        List<PlayerColors> activatedColors = ChooseControls.activatedPlayers.Where(p => p.Value).Select(p => p.Key).ToList();
        activatedColors.Add(color);
        activatedColors = activatedColors.OrderBy(c => (int)c).ToList();

        previouslySelectedPlayer = color;

        int index = activatedColors.IndexOf(color);
        StartCoroutine(ExecuteLevelLerp(index, activatedColors, color));
    }

    private IEnumerator ExecuteLevelLerp(int index, List<PlayerColors> activatedColors, PlayerColors color)
    {
        executeLevelLerpRunning = true;

        yield return new WaitUntil(() => !shouldLerpSmaller && !shouldLerpFromCircle && !shouldLerpToCircle);

        ChooseControls.activatedPlayers[color] = true;

        if (ChooseControls.activatedPlayers.Count(p => p.Value) > 3) // Normal level lerp
        {
            UpdateLerpLists(index, activatedColors);
            UpdateManagers(index, color);

            ReturnToNormalLevel();
        }
        else // Circle level lerp
        {
            StartFromCircleLerp(index, activatedColors);
        }

        executeLevelLerpRunning = false;
    }

    private void StartFromCircleLerp(int index, List<PlayerColors> activatedColors)
    {
        UpdateArqduts();
        shouldLerpFromCircle = true;
        lerpedAmount = 1;

        PlayerColors[] colors = ChooseControls.activatedPlayers.Where(o => o.Value).Select(i => i.Key).ToArray();
        BotSelection.botDifficulties[colors.Last()] = 0;
        Player p = playerManager.SpawnPlayer(pointManager.radius, colors, 0);

        p.playerOrder = index;
        p.color = activatedColors[index];
        p.transform.GetChild(0).GetComponent<SpriteRenderer>().color = MeshManager.materials[p.color].color;

        p.points = new Player.LeftRightPoints
        {
            left = innerPoints[innerPoints.Count - 2],
            right = innerPoints[innerPoints.Count - 1]
        };

        circleSpawningPlayer = p;
        arrowManager.AttachLeftArrow(p);
        HidePlayer(p.color);
        StartCoroutine(RevealPlayer(p));

        playerManager.SetFromCircleIndexes(index);

        meshManager.SetMaterials();
    }

    private void UpdateArqduts()
    {
        arqdutManager.DestroyAllArqduts();
        List<Vector2> arqdutSpawnPositions = new List<Vector2>(3) { innerPoints[0], innerPoints[innerPoints.Count - 2], innerPoints.Last() };
        arqdutManager.SpawnArqduts(arqdutSpawnPositions, levelCenter);
    }

    private void UpdateLerpLists(int index, List<PlayerColors> activatedColors)
    {
        innerPoints.Insert(index, index == innerPoints.Count ? innerPoints[0] : innerPoints[index]);
        outerPoints = pointManager.SpawnOuterPoints(innerPoints);

        innerLerpTo = pointManager.SpawnInnerPoints(activatedColors.Count, levelCenter);
        float rotateAmount = previousRotation + addPlayerRotationConstants[activatedColors.Count - 1][index];
        previousRotation = rotateAmount;
        pointManager.RotatePoints(innerLerpTo, rotateAmount);
        outerLerpTo = pointManager.SpawnOuterPoints(innerLerpTo);
    }

    private void UpdateManagers(int index, PlayerColors color)
    {
        arqdutManager.DestroyAllArqduts();
        arqdutManager.SpawnArqduts(innerPoints, levelCenter);
        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(innerPoints, outerPoints));
        meshManager.SetMaterials();
        playerManager.DestroyAllPlayers();
        playerManager.SpawnPlayers(pointManager.radius, ChooseControls.activatedPlayers.Where(o => o.Value).Select(i => i.Key).ToArray());
        arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.color == color));
        HidePlayer(color);
        StartCoroutine(RevealPlayer(PlayerManager.players.First(p => p.color == color)));

        playerManager.PlayersLookAtPoint(levelCenter);
        meshManager.AddIndicesAndDrawMesh(innerPoints.Count);
    }

    private IEnumerator Queue()
    {
        queueIsRunning = true;

        yield return new WaitUntil(() => !shouldLerpSmaller && !shouldLerpToNormal && !executeLevelLerpRunning && !shouldLerpFromCircle && !shouldLerpToCircle); // Wait until level is done lerping

        if (queue[0].lerpType) // Read next item in queue
        {
            StartAddPlayer(queue[0].color);
            lerpAmount = baseLerpAmount + lerpLargerModifier * queue.Count;
        }
        else
        {
            RemovePlayer(queue[0].color);
            lerpAmount += baseLerpAmount + lerpSmallerModifier * queue.Count;
        }

        queue.RemoveAt(0);

        if (queue.Count > 0) // Check if there are more items in queue
        {
            StartCoroutine(Queue());
            yield break;
        }

        lerpAmount = baseLerpAmount;

        queueIsRunning = false;
    }

    private void HidePlayer(PlayerColors color)
    {
        Player player = PlayerManager.players.First(p => p.color == color);
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

        if (player.transform.GetChild(0).childCount > 0)
            player.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }

    private void RemovePlayer(PlayerColors color)
    {
        if (ChooseControls.activatedPlayers.Count(p => p.Value) > 1 && PlayerManager.players.Select(p => p.color).Contains(color))
        {
            Player player = PlayerManager.players.First(p => p.color == color);

            if (ChooseControls.activatedPlayers.Count(p => p.Value) > 2) // Normal level removal
            {
                player.DestroyPlayer();
            }
            else // Lerp to circle
            {
                player.CircleDestroyPlayer();
            }

            playerToDestroy = player.playerOrder;
        }
        else if (PlayerManager.players.Select(p => p.color).Contains(color))
        {
            DestroyLevel();
        }
    }

    private void DestroyLevel()
    {
        arqdutManager.DestroyAllArqduts();
        playerManager.DestroyAllPlayers();
        meshManager.mesh.Clear();
        levelIsSpawned = false;
    }

    private IEnumerator RevealPlayer(Player player)
    {
        yield return new WaitUntil(() => Vector2.Distance(player.points.left, player.points.right) > disBeforePlayerSpawn);

        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        player.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
    }
}