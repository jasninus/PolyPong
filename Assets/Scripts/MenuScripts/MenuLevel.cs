using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

// TODO This whole system should probably be hooked up to the psychadelic size system instead. When a player is added, it's weight should start lerping from 0 to 1. When a player is removed, it's weight should lerp from 1 to 0. All players should be able to lerp at the same time. All players' weight will be normalized to calculate their actual weight
public class MenuLevel : LevelManager
{
    private bool shouldLerpFromCircle;

    private PlayerColor previouslySelectedPlayer;

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

    private DirectionArrows arrowManager;

    private readonly List<QueueItem> queue = new List<QueueItem>();

    private struct QueueItem
    {
        // False for smaller, true for bigger
        public bool lerpType;

        public PlayerColor Color;
    }

    private void SpawnLevel()
    {
        arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.Color == ChooseControls.controls.First(c => c.Value.rightKey == KeyCode.None).Key));
        arrowManager.SwitchArrowDirection();
        arrowManager.FlipArrow();
        StopAllCoroutines();
        shouldLerpFromCircle = false;
    }

    private void Update()
    {
        if (ChooseControls.playerStates.Count(p => p.Value != PlayerState.Deactivated) == 3)
        {
            SpawnLevel();
        }
    }

    private void UpdateArqduts()
    {
        arqdutManager.DestroyAllArqduts();
        List<Vector2> arqdutSpawnPositions = new List<Vector2>(3) { innerPoints[0], innerPoints[innerPoints.Count - 2], innerPoints.Last() };
        arqdutManager.SpawnArqduts(arqdutSpawnPositions, levelCenter);
    }

    private void UpdateLerpLists(int index, List<PlayerColor> activatedColors)
    {
        innerPoints.Insert(index, index == innerPoints.Count ? innerPoints[0] : innerPoints[index]);
        outerPoints = pointManager.SpawnOuterPoints(innerPoints);

        innerLerpTo = pointManager.SpawnInnerPoints(activatedColors.Count, levelCenter);
        float rotateAmount = previousRotation + addPlayerRotationConstants[activatedColors.Count - 1][index];
        previousRotation = rotateAmount;
        pointManager.RotatePoints(innerLerpTo, rotateAmount);
        outerLerpTo = pointManager.SpawnOuterPoints(innerLerpTo);
    }

    private void UpdateManagers(int index, PlayerColor color)
    {
        arqdutManager.DestroyAllArqduts();
        arqdutManager.SpawnArqduts(innerPoints, levelCenter);
        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(innerPoints, outerPoints));
        meshManager.SetMaterials();
        playerManager.DestroyAllPlayers();
        playerManager.SpawnPlayers(pointManager.radius, ChooseControls.playerStates.Where(o => o.Value != PlayerState.Deactivated).Select(i => i.Key).ToArray());
        arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.Color == color));
        HidePlayer(color);
        StartCoroutine(RevealPlayer(PlayerManager.players.First(p => p.Color == color)));

        playerManager.PlayersLookAtPoint(levelCenter);
        meshManager.AddIndicesAndDrawMesh(innerPoints.Count);
    }

    private void HidePlayer(PlayerColor color)
    {
        Player player = PlayerManager.players.First(p => p.Color == color);
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

        if (player.transform.GetChild(0).childCount > 0)
            player.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }

    private IEnumerator RevealPlayer(Player player)
    {
        yield return new WaitUntil(() => Vector2.Distance(player.points.left, player.points.right) > disBeforePlayerSpawn);

        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        player.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
    }
}

#region MightBeUsefulForCircle

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

#endregion MightBeUsefulForCircle