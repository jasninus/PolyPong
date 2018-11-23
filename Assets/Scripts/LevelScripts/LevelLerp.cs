using System;
using System.Linq;
using UnityEngine;

public class LevelLerp : MonoBehaviour
{
    private readonly InGameManager _inGameManager;
    private readonly LevelPoints pointManager;
    private readonly PointLerp lerpManager;
    private readonly MeshManager meshManager;
    private readonly PlayerManager playerManager;
    private readonly ArqdutManager arqdutManager;
    private readonly GameStart _gameManager;

    private int lerpPlayerNumber;
    public readonly int[] rotationConstants = { 0, 0, 0, 30, 18, 12 } /* constants used in rotation calculations */, joiningPoints = new int[2];

    public LevelLerp(InGameManager inGameManager, PointLerp lerpManager, LevelPoints pointManager, MeshManager meshManager, PlayerManager playerManager, ArqdutManager arqdutManager, GameStart gameManager)
    {
        this._inGameManager = inGameManager;
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

        InGameManager.innerLerpFrom = InGameManager.innerPoints;
        InGameManager.outerLerpFrom = InGameManager.outerPoints;

        // TODO there was error about index being out of bounds of array after ending game in circle. This method shouldn't even be called after player dies in circle
        float rotateAmount = 360 / (InGameManager.innerPoints.Count * 2) - rotationConstants[InGameManager.innerPoints.Count - 1] * playerOrder + _inGameManager.previousRotation; // Calculate rotation
        _inGameManager.previousRotation = rotateAmount;

        InGameManager.innerLerpTo = pointManager.SpawnInnerPoints(InGameManager.innerPoints.Count - 1, _inGameManager.levelCenter);
        pointManager.RotatePoints(InGameManager.innerLerpTo, rotateAmount); // Rotate inner points
        InGameManager.outerLerpTo = pointManager.SpawnOuterPoints(InGameManager.innerLerpTo);

        joiningPoints[0] = playerOrder;
        joiningPoints[1] = playerOrder + 1;

        _gameManager.StartCountdown(_inGameManager.levelCenter);
    }

    private void LerpPointsSmaller(int playerOrder, PointLerp lerpManager)
    {
        InGameManager.innerPoints = lerpManager.LerpLevelSmaller(InGameManager.innerLerpFrom, InGameManager.innerLerpTo, _inGameManager.lerpAmount + _inGameManager.lerpedAmount, joiningPoints, playerOrder);
        InGameManager.outerPoints = lerpManager.LerpLevelSmaller(InGameManager.outerLerpFrom, InGameManager.outerLerpTo, _inGameManager.lerpAmount + _inGameManager.lerpedAmount, joiningPoints, playerOrder);

        _inGameManager.lerpedAmount += _inGameManager.lerpAmount;
    }

    private void FinishLerping()
    {
        _inGameManager.DestroyPlayer(InGameManager.playerToDestroy);

        InGameManager.innerPoints = InGameManager.innerLerpTo;
        InGameManager.outerPoints = InGameManager.outerLerpTo;

        InGameManager.shouldLerpSmaller = false;

        meshManager.SetMaterials();
        _inGameManager.lerpedAmount = 0;
        playerManager.UpdatePlayerPositions();
    }

    public void LerpLevel()
    {
        LerpPointsSmaller(lerpPlayerNumber, lerpManager);

        if (_inGameManager.lerpedAmount >= 1) // Finished lerping
        {
            FinishLerping();
        }

        playerManager.PlayersLookAtPoint(_inGameManager.levelCenter);
        playerManager.UpdatePlayerPositions();
        arqdutManager.UpdateArqdutPositions(InGameManager.innerPoints, _inGameManager.levelCenter);
        _inGameManager.DrawMesh(ChooseControls.activatedPlayers.Count(i => i.Value) + Convert.ToByte(InGameManager.shouldLerpSmaller));

        InGameManager.shouldSetIndices = false;
    }
}