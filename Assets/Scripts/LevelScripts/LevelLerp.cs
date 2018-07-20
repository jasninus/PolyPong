using System;
using System.Linq;
using UnityEngine;

public class LevelLerp : MonoBehaviour
{
    private readonly LevelManager levelManager;
    private readonly LevelPoints pointManager;
    private readonly PointLerp lerpManager;
    private readonly MeshManager meshManager;
    private readonly PlayerManager playerManager;
    private readonly ArqdutManager arqdutManager;
    private readonly GameStart _gameManager;

    private int lerpPlayerNumber;
    private readonly int[] rotationConstants = { 0, 0, 0, 30, 18, 12 } /* constants used in rotation calculations */, joiningPoints = new int[2];

    public LevelLerp(LevelManager levelManager, PointLerp lerpManager, LevelPoints pointManager, MeshManager meshManager, PlayerManager playerManager, ArqdutManager arqdutManager, GameStart gameManager)
    {
        this.levelManager = levelManager;
        this.lerpManager = lerpManager;
        this.pointManager = pointManager;
        this.meshManager = meshManager;
        this.playerManager = playerManager;
        this.arqdutManager = arqdutManager;
        this._gameManager = gameManager;
    }

    public void StartLerpSmaller(int playerOrder)
    {
        lerpPlayerNumber = playerOrder;

        LevelManager.innerLerpFrom = LevelManager.innerPoints;
        LevelManager.outerLerpFrom = LevelManager.outerPoints;

        float rotateAmount = 360 / (LevelManager.innerPoints.Count * 2) - rotationConstants[LevelManager.innerPoints.Count - 1] * playerOrder + levelManager.previousRotation; // Calculate rotation
        levelManager.previousRotation = rotateAmount;

        LevelManager.innerLerpTo = pointManager.SpawnInnerPoints(LevelManager.innerPoints.Count - 1, levelManager.levelCenter);
        pointManager.RotatePoints(LevelManager.innerLerpTo, rotateAmount); // Rotate inner points
        LevelManager.outerLerpTo = pointManager.SpawnOuterPoints(LevelManager.innerLerpTo);

        joiningPoints[0] = playerOrder;
        joiningPoints[1] = playerOrder + 1;

        _gameManager.StartCountdown(levelManager.levelCenter);
    }

    private void LerpPointsSmaller(int playerOrder, PointLerp lerpManager)
    {
        LevelManager.innerPoints = lerpManager.LerpLevelSmaller(LevelManager.innerLerpFrom, LevelManager.innerLerpTo, levelManager.lerpAmount + levelManager.lerpedAmount, joiningPoints, playerOrder);
        LevelManager.outerPoints = lerpManager.LerpLevelSmaller(LevelManager.outerLerpFrom, LevelManager.outerLerpTo, levelManager.lerpAmount + levelManager.lerpedAmount, joiningPoints, playerOrder);

        levelManager.lerpedAmount += levelManager.lerpAmount;
    }

    private void FinishLerping()
    {
        levelManager.DestroyPlayer(LevelManager.playerToDestroy);
        LevelManager.shouldLerpSmaller = false;

        LevelManager.innerPoints = LevelManager.innerLerpTo;
        LevelManager.outerPoints = LevelManager.outerLerpTo;
        meshManager.SetMaterials();
        levelManager.lerpedAmount = 0;
        playerManager.UpdatePlayerPositions();
    }

    public void LerpLevel()
    {
        LerpPointsSmaller(lerpPlayerNumber, lerpManager);

        if (levelManager.lerpedAmount >= 1) // Finished lerping
        {
            FinishLerping();
        }

        playerManager.PlayersLookAtPoint(levelManager.levelCenter);
        playerManager.UpdatePlayerPositions();
        arqdutManager.UpdateArqdutPositions(LevelManager.innerPoints, levelManager.levelCenter);
        levelManager.DrawMesh(ChooseControls.activatedPlayers.Count(i => i.Value) + Convert.ToByte(LevelManager.shouldLerpSmaller));

        LevelManager.shouldSetIndices = false;
    }
}