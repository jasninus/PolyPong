using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

// TODO This whole system should probably be hooked up to the psychadelic size system instead. When a player is added, it's weight should start lerping from 0 to 1. When a player is removed, it's weight should lerp from 1 to 0. All players should be able to lerp at the same time. All players' weight will be normalized to calculate their actual weight
public class MenuLevelManager : LevelManager
{
    public static bool levelIsSpawned;
    private bool queueIsRunning, executeLevelLerpRunning;

    private PlayerColors previouslySelectedPlayer;

    private readonly int[][] addPlayerRotationConstants = { new[] { 0 }, new[] { 0 }, new[] { 0 }, new[] { -45, -15, 15, 45 }, new[] { -36, -18, 0, 18, 36 }, new[] { -30, -18, -6, 6, 18, 30 } };

    [SerializeField] private float disBeforePlayerSpawn;

    private List<QueueItem> queue = new List<QueueItem>();

    private DirectionArrows arrowManager;

    private struct QueueItem
    {
        // 0 for smaller, 1 for bigger
        public bool lerpType;

        public PlayerColors color;
    }

    protected override void Awake()
    {
        base.Awake();

        arrowManager = GetComponent<DirectionArrows>();

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
        levelSpawner.SpawnLevel(3);
        arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.color == ChooseControls.controls.First(c => c.Value.rightKey == KeyCode.None).Key));
        arrowManager.SwitchArrowDirection();
        levelIsSpawned = true;
    }

    private void Update()
    {
        if (ChooseControls.activatedPlayers.Count(p => p.Value) == 3 && !levelIsSpawned) // The first
        {
            SpawnLevel();
        }

        if (queue.Count > 0 && !queueIsRunning) // Start queue if queue is not running and there are queued items
        {
            StartCoroutine(Queue());
        }
    }

    private void QueueAddPlayer(PlayerColors color)
    {
        queue.Add(new QueueItem { lerpType = true, color = color });
    }

    private void QueueRemovePlayer(PlayerColors color)
    {
        queue.Add(new QueueItem { lerpType = false, color = color });
    }

    private void StartAddPlayer(PlayerColors color)
    {
        UpdateActivatedPlayers(color);

        if (!ChooseControls.activatedPlayers[color] && ChooseControls.activatedPlayers.Count(p => p.Value) > 2)
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

        yield return new WaitUntil(() => !shouldLerpSmaller);

        ChooseControls.activatedPlayers[color] = true;

        UpdateLerpLists(index, activatedColors);
        UpdateManagers(index, color);

        ReturnToNormalLevel();

        executeLevelLerpRunning = false;
    }

    private IEnumerator Queue()
    {
        queueIsRunning = true;

        yield return new WaitUntil(() => !shouldLerpSmaller && !shouldLerpToNormal && !executeLevelLerpRunning); // Wait until level is done lerping

        if (queue[0].lerpType) // Read next item in queue
        {
            StartAddPlayer(queue[0].color);
        }
        else
        {
            RemovePlayer(queue[0].color);
        }

        queue.RemoveAt(0);

        if (queue.Count > 0) // Check if there are more items in queue
        {
            StartCoroutine(Queue());
            yield break;
        }

        queueIsRunning = false;
    }

    private void UpdateActivatedPlayers(PlayerColors selectedColor)
    {
        if (ChooseControls.controls[previouslySelectedPlayer].leftKey == KeyCode.None)
        {
            ChooseControls.activatedPlayers[previouslySelectedPlayer] = false; // TODO this gets run and sets yellow to inactive, when it shouldn't
            RemovePlayer(previouslySelectedPlayer);
        }
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
        playerManager.SpawnPlayers(pointManager.radius);
        arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.color == color));
        HidePlayer(color);
        StartCoroutine(RevealPlayer(PlayerManager.players.First(p => p.color == color)));
    }

    private void HidePlayer(PlayerColors color)
    {
        Player player = PlayerManager.players.First(p => p.color == color);
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        player.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }

    private void RemovePlayer(PlayerColors color)
    {
        if (ChooseControls.activatedPlayers.Count(p => p.Value) > 2 && PlayerManager.players.Select(p => p.color).Contains(color))
        {
            PlayerManager.players.Where(p => p.color == color).ElementAt(0).DestroyPlayer();
            playerToDestroy = PlayerManager.players.Where(p => p.color == color).ElementAt(0).playerOrder;
        }
    }

    private IEnumerator RevealPlayer(Player player)
    {
        yield return new WaitUntil(() => Vector2.Distance(player.points.left, player.points.right) > disBeforePlayerSpawn);

        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        player.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

        // A player should be spawned as soon as there is space for it to be spawned on it's part of the level. The arrow indicating the currently selected control direction should be spawned as soon as there is space to spawn it
    }
}