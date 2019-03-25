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

    private PlayerColor _circleSpawnedPlayerColor;

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
        LevelManager.innerLerpFrom = pointManager.SpawnCircleLerpPoints(PlayerManager.players[playerOrder].points, LevelManager.innerPoints, _inGameManager.levelCenter, circleEdges);
        LevelManager.outerLerpFrom = LevelManager.innerLerpFrom.GetRange(LevelManager.innerLerpFrom.Count / 2, LevelManager.innerLerpFrom.Count / 2);
        LevelManager.innerLerpFrom.RemoveRange(LevelManager.innerLerpFrom.Count / 2, LevelManager.innerLerpFrom.Count / 2);

        Vector2 avgVector = ((PlayerManager.players[playerOrder].points.left - _inGameManager.levelCenter) + (PlayerManager.players[playerOrder].points.right - _inGameManager.levelCenter)) / 2;
        float rotateAmount = avgVector.y < 0 ? 360 - Mathf.Acos(avgVector.normalized.x) * Mathf.Rad2Deg : Mathf.Acos(avgVector.normalized.x) * Mathf.Rad2Deg;

        // Assign to lists
        LevelManager.innerLerpTo = pointManager.SpawnInnerPoints(circleEdges * 2, _inGameManager.levelCenter);
        pointManager.RotatePoints(LevelManager.innerLerpTo, rotateAmount);
        LevelManager.outerLerpTo = pointManager.GetOuterPoints(LevelManager.innerLerpTo);

        //meshManager.SetMaterials();

        playerManager.CircleSetPlayerPoints(LevelManager.innerLerpTo, playerOrder);
        _gameManager.StartCountdown(_inGameManager.levelCenter);
    }

    public void LerpToCircle()
    {
        LevelManager.innerPoints = lerpManager.LerpToCircle(LevelManager.innerLerpFrom, LevelManager.innerLerpTo, _inGameManager.lerpedAmount);
        LevelManager.outerPoints = lerpManager.LerpToCircle(LevelManager.outerLerpFrom, LevelManager.outerLerpTo, _inGameManager.lerpedAmount);

        playerManager.CircleUpdatePlayerPosition(_inGameManager.lerpedAmount);
        playerManager.PlayersLookAtPoint(_inGameManager.levelCenter);
        arqdutManager.CircleUpdateArqdutPosition(LevelManager.innerPoints, _inGameManager.levelCenter);

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(LevelManager.innerPoints, LevelManager.outerPoints));

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

        LevelManager.innerPoints = pointManager.SpawnInnerPoints(3, _inGameManager.levelCenter); // Problem with rotation
        LevelManager.outerPoints = pointManager.GetOuterPoints(LevelManager.innerPoints);

        meshManager.mesh.Clear();
        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(LevelManager.innerPoints, LevelManager.outerPoints));
        meshManager.SetMaterials(ChooseControls.playerStates.Where(i => i.Value != PlayerState.Deactivated).Select(item => item.Key).ToArray());
        meshManager.AddIndicesAndDrawMesh(3);

        _circleSpawnedPlayerColor = _inGameManager.circleSpawningPlayer.Color;

        PlayerColor[] activatedColor = PlayerManager.players.Select(p => p.Color).OrderBy(i => (int)i).ToArray();

        playerManager.DestroyAllPlayers();
        playerManager.SpawnPlayers(pointManager.radius, activatedColor);
        playerManager.PlayersLookAtPoint(_inGameManager.levelCenter);

        arrowManager.AttachLeftArrow(PlayerManager.players.First(p => p.Color == _circleSpawnedPlayerColor));
    }
}