using System.Collections;
using System.Collections.Generic;
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

    private int lerpPlayerNumber;
    private readonly int[] rotationConstants = { 0, 0, 0, 30, 18, 12 }, joiningPoints = new int[2];
    private readonly int circleEdges = 20;

    public LevelLerpCircle(LevelManager levelManager, PointLerp lerpManager, LevelPoints pointManager, MeshManager meshManager, PlayerManager playerManager, ArqdutManager arqdutManager, GameStart gameManager)
    {
        this.levelManager = levelManager;
        this.lerpManager = lerpManager;
        this.pointManager = pointManager;
        this.meshManager = meshManager;
        this.playerManager = playerManager;
        this.arqdutManager = arqdutManager;
        this._gameManager = gameManager;
    }

    public void StartLerpToCircle(int playerOrder) // TODO this should probably take more parameters instead of referring directly to variables in levelManager
    {
        // Assign from lists
        LevelManager.innerLerpFrom = pointManager.SpawnCircleLerpPoints(PlayerManager.players[playerOrder].points, LevelManager.innerPoints, levelManager.levelCenter, circleEdges);
        LevelManager.outerLerpFrom = LevelManager.innerLerpFrom.GetRange(LevelManager.innerLerpFrom.Count / 2, LevelManager.innerLerpFrom.Count / 2);
        LevelManager.innerLerpFrom.RemoveRange(LevelManager.innerLerpFrom.Count / 2, LevelManager.innerLerpFrom.Count / 2);

        float rotateAmount = ((PlayerManager.players[playerOrder].points.left + PlayerManager.players[playerOrder].points.right) / 2).y < 0 ?
            360 - Mathf.Acos(((PlayerManager.players[playerOrder].points.left + PlayerManager.players[playerOrder].points.right) / 2).normalized.x) * Mathf.Rad2Deg :
            Mathf.Acos(((PlayerManager.players[playerOrder].points.left + PlayerManager.players[playerOrder].points.right) / 2).normalized.x) * Mathf.Rad2Deg; // TODO should probably extract the vector average to keep it shorter

        // Assign to lists
        LevelManager.innerLerpTo = pointManager.SpawnInnerPoints(circleEdges * 2, levelManager.levelCenter);
        pointManager.RotatePoints(LevelManager.innerLerpTo, rotateAmount);
        LevelManager.outerLerpTo = pointManager.SpawnOuterPoints(LevelManager.innerLerpTo);

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

        levelManager.lerpedAmount += levelManager.lerpAmount;
    }

    public void LerpCircle()
    {
        LerpToCircle();

        if (levelManager.lerpedAmount >= 1 + levelManager.lerpAmount)
        {
            LevelManager.shouldLerpToCircle = false;
            levelManager.lerpedAmount = 0;

            levelManager.DestroyPlayer(LevelManager.playerToDestroy);
            playerManager.SpawnCircleMovementObjs(levelManager.levelCenter);
            playerManager.SpawnCircleColliders(LevelManager.playerToDestroy);
            LevelManager.isCircle = true;
        }

        if (LevelManager.shouldSetIndices)
        {
            meshManager.Circle.CircleAddIndicesAndDrawMesh(circleEdges, PlayerManager.players[LevelManager.playerToDestroy].playerOrder);
            LevelManager.shouldSetIndices = false;
        }
    }
}