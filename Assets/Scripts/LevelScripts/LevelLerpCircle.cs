using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLerpCircle : MonoBehaviour
{
    private readonly InGameManager _inGameManager;
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

    public LevelLerpCircle(InGameManager inGameManager, PointLerp lerpManager, LevelPoints pointManager, MeshManager meshManager, PlayerManager playerManager, ArqdutManager arqdutManager, GameStart gameManager, DirectionArrows arrowManager)
    {
        this._inGameManager = inGameManager;
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
        InGameManager.innerLerpFrom = pointManager.SpawnCircleLerpPoints(PlayerManager.players[playerOrder].points, InGameManager.innerPoints, _inGameManager.levelCenter, circleEdges);
        InGameManager.outerLerpFrom = InGameManager.innerLerpFrom.GetRange(InGameManager.innerLerpFrom.Count / 2, InGameManager.innerLerpFrom.Count / 2);
        InGameManager.innerLerpFrom.RemoveRange(InGameManager.innerLerpFrom.Count / 2, InGameManager.innerLerpFrom.Count / 2);

        Vector2 avgVector = ((PlayerManager.players[playerOrder].points.left - _inGameManager.levelCenter) + (PlayerManager.players[playerOrder].points.right - _inGameManager.levelCenter)) / 2;
        float rotateAmount = avgVector.y < 0 ? 360 - Mathf.Acos(avgVector.normalized.x) * Mathf.Rad2Deg : Mathf.Acos(avgVector.normalized.x) * Mathf.Rad2Deg;

        // Assign to lists
        InGameManager.innerLerpTo = pointManager.SpawnInnerPoints(circleEdges * 2, _inGameManager.levelCenter);
        pointManager.RotatePoints(InGameManager.innerLerpTo, rotateAmount);
        InGameManager.outerLerpTo = pointManager.SpawnOuterPoints(InGameManager.innerLerpTo);

        //meshManager.SetMaterials();

        playerManager.CircleSetPlayerPoints(InGameManager.innerLerpTo, playerOrder);
        _gameManager.StartCountdown(_inGameManager.levelCenter);
    }

    public void LerpToCircle()
    {
        InGameManager.innerPoints = lerpManager.LerpToCircle(InGameManager.innerLerpFrom, InGameManager.innerLerpTo, _inGameManager.lerpedAmount);
        InGameManager.outerPoints = lerpManager.LerpToCircle(InGameManager.outerLerpFrom, InGameManager.outerLerpTo, _inGameManager.lerpedAmount);

        playerManager.CircleUpdatePlayerPosition(_inGameManager.lerpedAmount);
        playerManager.PlayersLookAtPoint(_inGameManager.levelCenter);
        arqdutManager.CircleUpdateArqdutPosition(InGameManager.innerPoints, _inGameManager.levelCenter);

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(InGameManager.innerPoints, InGameManager.outerPoints));

        _inGameManager.lerpedAmount += _inGameManager.shouldLerpFromCircle ? -_inGameManager.lerpAmount : _inGameManager.lerpAmount;
    }

    public void LerpCircle()
    {
        LerpToCircle();

        if (_inGameManager.lerpedAmount >= 1 + _inGameManager.lerpAmount)
        {
            FinishToLerp();
        }

        if (InGameManager.shouldSetIndices)
        {
            meshManager.Circle.CircleAddIndicesAndDrawMesh(circleEdges, PlayerManager.players[InGameManager.playerToDestroy].playerOrder);
            InGameManager.shouldSetIndices = false;
        }
    }

    private void FinishToLerp()
    {
        InGameManager.shouldLerpToCircle = false;
        _inGameManager.lerpedAmount = 0;

        _inGameManager.DestroyPlayer(InGameManager.playerToDestroy);

        if (ChooseControls.gameStarted)
        {
            playerManager.SpawnCircleMovementObjs(_inGameManager.levelCenter);
            playerManager.SpawnCircleColliders(InGameManager.playerToDestroy);
            InGameManager.isCircle = true;
        }
    }

    public void LerpFromCircle()
    {
        LerpToCircle();

        if (_inGameManager.lerpedAmount <= 0 - _inGameManager.lerpAmount)
        {
            FinishFromLerp();
        }

        if (InGameManager.shouldSetIndices)
        {
            meshManager.Circle.CircleAddIndicesAndDrawMesh(circleEdges, PlayerManager.players[InGameManager.playerToDestroy].playerOrder);
            InGameManager.shouldSetIndices = false;
        }
    }

    private void FinishFromLerp()
    {
        _inGameManager.shouldLerpFromCircle = false;
        _inGameManager.lerpedAmount = 0;

        InGameManager.innerPoints = pointManager.SpawnInnerPoints(3, _inGameManager.levelCenter); // Problem with rotation
        InGameManager.outerPoints = pointManager.SpawnOuterPoints(InGameManager.innerPoints);

        meshManager.mesh.Clear();
        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(InGameManager.innerPoints, InGameManager.outerPoints));
        meshManager.SetMaterials();
        meshManager.AddIndicesAndDrawMesh(3);

        circleSpawnedPlayerColor = _inGameManager.circleSpawningPlayer.color;

        PlayerColors[] activatedColors = PlayerManager.players.Select(p => p.color).OrderBy(i => (int)i).ToArray();

        playerManager.DestroyAllPlayers();
        playerManager.SpawnPlayers(pointManager.radius, activatedColors);
        playerManager.PlayersLookAtPoint(_inGameManager.levelCenter);

        arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.color == circleSpawnedPlayerColor));
    }
}