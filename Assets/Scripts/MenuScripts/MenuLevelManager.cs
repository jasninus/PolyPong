using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuLevelManager : LevelManager
{
    private bool levelIsSpawned;

    private PlayerColors previouslySelectedPlayer;

    protected override void Awake()
    {
        base.Awake();

        ChooseControls.PlayerAmountIncreased += AddPlayer;
        ChooseControls.PlayerAmountDecreased += RemovePlayer;
    }

    protected override void Start()
    {
        // REASON Here to make sure level is not spawned immediately
    }

    private void SpawnLevel()
    {
        levelSpawner.SpawnLevel(3);
        levelIsSpawned = true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (ChooseControls.activatedPlayers.Count(p => p.Value) == 3 && !levelIsSpawned)
        {
            SpawnLevel();
        }
    }

    private void AddPlayer(PlayerColors color) // The player that is currently selected, should be displayed on the example level
    {
        UpdateActivatedPlayers();

        if (!ChooseControls.activatedPlayers[color] && ChooseControls.activatedPlayers.Count(p => p.Value) > 2)
        {
            // Find out what innerPoint should be duplicated to accomodate new player
            List<PlayerColors> activatedColors = ChooseControls.activatedPlayers.Where(p => p.Value).Select(p => p.Key).ToList();
            activatedColors.Add(color);
            activatedColors = activatedColors.OrderBy(c => (int)c).ToList();

            previouslySelectedPlayer = color;
            ChooseControls.activatedPlayers[color] = true;

            int index = activatedColors.IndexOf(color);
            StartCoroutine(ExecuteLevelLerp(index, activatedColors));
        }
    }

    private IEnumerator ExecuteLevelLerp(int index, List<PlayerColors> activatedColors)
    {
        yield return new WaitUntil(() => !shouldLerpSmaller);

        UpdateLerpLists(index, activatedColors);
        UpdateManagers(index);

        ReturnToNormalLevel();
    }

    private void UpdateActivatedPlayers()
    {
        if (ChooseControls.controls[previouslySelectedPlayer].rightKey == KeyCode.None)
        {
            ChooseControls.activatedPlayers[previouslySelectedPlayer] = false;
            RemovePlayer(previouslySelectedPlayer);
        }
    }

    private void UpdateLerpLists(int index, List<PlayerColors> activatedColors)
    {
        Debug.Log(shouldLerpSmaller);

        // TODO if shouldLerpSmaller is true, that means that it is already lerping smaller

        innerPoints.Insert(index, index == innerPoints.Count ? innerPoints[0] : innerPoints[index]);
        outerPoints = pointManager.SpawnOuterPoints(innerPoints);

        innerLerpTo = pointManager.SpawnInnerPoints(activatedColors.Count, levelCenter);
        outerLerpTo = pointManager.SpawnOuterPoints(innerLerpTo);
    }

    private void UpdateManagers(int index)
    {
        arqdutManager.DestroyAllArqduts();
        arqdutManager.SpawnArqduts(innerPoints, levelCenter);
        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(innerPoints, outerPoints));
        meshManager.SetMaterials();
        playerManager.DestroyAllPlayers();
        playerManager.SpawnPlayers(pointManager.radius);
    }

    private void RemovePlayer(PlayerColors color)
    {
        if (ChooseControls.activatedPlayers.Count(p => p.Value) > 2 && PlayerManager.players.Select(p => p.color).Contains(color))
        {
            PlayerManager.players.Where(p => p.color == color).ElementAt(0).DestroyPlayer();
            playerToDestroy = PlayerManager.players.Where(p => p.color == color).ElementAt(0).playerOrder;
        }
    }

    private void SpawnPlayer()
    {
        // A player should be spawned as soon as there is space for it to be spawned on it's part of the level. The arrow indicating the currently selected control direction should be spawned as soon as there is space to spawn it
    }
}