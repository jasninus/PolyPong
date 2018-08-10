using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLerpCircle : MonoBehaviour
{
    private readonly LevelManager levelManager;
    private readonly PointLerp lerpManager;
    private readonly LevelPoints pointManager;
    private readonly MeshManager meshManager;
    private readonly PlayerManager playerManager;
    private readonly ArqdutManager arqdutManager;
    private readonly GameStart _gameManager;
    private readonly DirectionArrows arrowManager;

    private int lerpPlayerNumber;
    private readonly int[] rotationConstants = { 0, 0, 0, 30, 18, 12 }, joiningPoints = new int[2];
    private readonly int circleEdges = 20;

    private PlayerColors circleSpawnedPlayerColor;

    public LevelLerpCircle(LevelManager levelManager, PointLerp lerpManager, LevelPoints pointManager, MeshManager meshManager, PlayerManager playerManager, ArqdutManager arqdutManager, GameStart gameManager, DirectionArrows arrowManager)
    {
        this.levelManager = levelManager;
        this.lerpManager = lerpManager;
        this.pointManager = pointManager;
        this.meshManager = meshManager;
        this.playerManager = playerManager;
        this.arqdutManager = arqdutManager;
        this._gameManager = gameManager;
        this.arrowManager = arrowManager;
    }

    public void StartLerpToCircle(int playerOrder)
    {
        // Assign from lists
        LevelManager.innerLerpFrom = pointManager.SpawnCircleLerpPoints(PlayerManager.players[playerOrder].points, LevelManager.innerPoints, levelManager.levelCenter, circleEdges);
        LevelManager.outerLerpFrom = LevelManager.innerLerpFrom.GetRange(LevelManager.innerLerpFrom.Count / 2, LevelManager.innerLerpFrom.Count / 2);
        LevelManager.innerLerpFrom.RemoveRange(LevelManager.innerLerpFrom.Count / 2, LevelManager.innerLerpFrom.Count / 2);

        Vector2 avgVector = ((PlayerManager.players[playerOrder].points.left - levelManager.levelCenter) + (PlayerManager.players[playerOrder].points.right - levelManager.levelCenter)) / 2;
        float rotateAmount = avgVector.y < 0 ? 360 - Mathf.Acos(avgVector.normalized.x) * Mathf.Rad2Deg : Mathf.Acos(avgVector.normalized.x) * Mathf.Rad2Deg;

        // Assign to lists
        LevelManager.innerLerpTo = pointManager.SpawnInnerPoints(circleEdges * 2, levelManager.levelCenter);
        pointManager.RotatePoints(LevelManager.innerLerpTo, rotateAmount);
        LevelManager.outerLerpTo = pointManager.SpawnOuterPoints(LevelManager.innerLerpTo);

        //meshManager.SetMaterials();

        playerManager.CircleSetPlayerPoints(LevelManager.innerLerpTo, playerOrder);
        _gameManager.StartCountdown(levelManager.levelCenter);
    }

    public void LerpToCircle()
    {
        LevelManager.innerPoints = lerpManager.LerpToCircle(LevelManager.innerLerpFrom, LevelManager.innerLerpTo, levelManager.lerpedAmount);
        LevelManager.outerPoints = lerpManager.LerpToCircle(LevelManager.outerLerpFrom, LevelManager.outerLerpTo, levelManager.lerpedAmount);

        playerManager.CircleUpdatePlayerPosition(levelManager.lerpedAmount);
        playerManager.PlayersLookAtPoint(levelManager.levelCenter);
        arqdutManager.CircleUpdateArqdutPosition(LevelManager.innerPoints, levelManager.levelCenter);

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(LevelManager.innerPoints, LevelManager.outerPoints));

        levelManager.lerpedAmount += levelManager.shouldLerpFromCircle ? -levelManager.lerpAmount : levelManager.lerpAmount;
    }

    public void LerpCircle()
    {
        LerpToCircle();

        if (levelManager.lerpedAmount >= 1 + levelManager.lerpAmount)
        {
            FinishToLerp();
        }

        if (LevelManager.shouldSetIndices)
        {
            meshManager.Circle.CircleAddIndicesAndDrawMesh(circleEdges, PlayerManager.players[LevelManager.playerToDestroy].playerOrder);
            LevelManager.shouldSetIndices = false;
        }
    }

    private void FinishToLerp()
    {
        LevelManager.shouldLerpToCircle = false;
        levelManager.lerpedAmount = 0;

        levelManager.DestroyPlayer(LevelManager.playerToDestroy);

        if (ChooseControls.gameStarted)
        {
            playerManager.SpawnCircleMovementObjs(levelManager.levelCenter);
            playerManager.SpawnCircleColliders(LevelManager.playerToDestroy);
            LevelManager.isCircle = true;
        }
    }

    public void LerpFromCircle()
    {
        LerpToCircle();

        if (levelManager.lerpedAmount <= 0 - levelManager.lerpAmount)
        {
            FinishFromLerp();
        }

        if (LevelManager.shouldSetIndices)
        {
            meshManager.Circle.CircleAddIndicesAndDrawMesh(circleEdges, PlayerManager.players[LevelManager.playerToDestroy].playerOrder);
            LevelManager.shouldSetIndices = false;
        }
    }

    private void FinishFromLerp()
    {
        levelManager.shouldLerpFromCircle = false;
        levelManager.lerpedAmount = 0;

        LevelManager.innerPoints = pointManager.SpawnInnerPoints(3, levelManager.levelCenter); // Problem with rotation
        LevelManager.outerPoints = pointManager.SpawnOuterPoints(LevelManager.innerPoints);

        meshManager.mesh.Clear();
        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(LevelManager.innerPoints, LevelManager.outerPoints));
        meshManager.SetMaterials();
        meshManager.AddIndicesAndDrawMesh(3);

        circleSpawnedPlayerColor = levelManager.circleSpawningPlayer.color;

        PlayerColors[] activatedColors = PlayerManager.players.Select(p => p.color).OrderBy(i => (int)i).ToArray();

        playerManager.DestroyAllPlayers();
        playerManager.SpawnPlayers(pointManager.radius, activatedColors);
        playerManager.PlayersLookAtPoint(levelManager.levelCenter);

        arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.color == circleSpawnedPlayerColor));
    }
}