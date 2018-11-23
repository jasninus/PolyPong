using System.Linq;
using UnityEngine;

public class LevelSpawner
{
    private readonly InGameManager _inGameManager;
    private readonly LevelLerpCircle circleLerpManager;
    private readonly LevelPoints pointManager;
    private readonly PlayerManager playerManager;
    private readonly MeshManager meshManager;
    private readonly ArqdutManager arqdutManager;
    private readonly GameStart _gameManager;

    public LevelSpawner(InGameManager inGameManager, LevelLerpCircle circleLerpManager, LevelPoints pointManager, PlayerManager playerManager, MeshManager meshManager, ArqdutManager arqdutManager, GameStart gameManager)
    {
        this._inGameManager = inGameManager;
        this.circleLerpManager = circleLerpManager;
        this.pointManager = pointManager;
        this.playerManager = playerManager;
        this.meshManager = meshManager;
        this.arqdutManager = arqdutManager;
        this._gameManager = gameManager;
    }

    /// <summary>
    /// Spawns the circle immediately
    /// </summary>
    /// <remarks>Actually spawns an extra player and lerps immediately to a circle</remarks>
    public void SpawnCircle()
    {
        PlayerColor extraPlayerColor = ChooseControls.playerStates.First(i => i.Value == PlayerState.Deactivated).Key;
        ChooseControls.playerStates[extraPlayerColor] = PlayerState.Activated;

        SpawnLevel(3);
        InGameManager.playerToDestroy = PlayerManager.players.First(p => p.Color == extraPlayerColor).playerOrder;
        _inGameManager.StartDestroyPlayer();
        _inGameManager.lerpedAmount = 1 - 2 * _inGameManager.lerpAmount;
        circleLerpManager.LerpToCircle();

        Object.Destroy(GameObject.FindWithTag("Ball")); // Destroy extra ball

        ChooseControls.playerStates[extraPlayerColor] = PlayerState.Deactivated;
    }

    /// <summary>
    /// Calls all methods necessary for spawning the initial level
    /// </summary>
    public void SpawnLevel(int corners)
    {
        LevelManager.innerPoints = pointManager.SpawnInnerPoints(corners, _inGameManager.levelCenter);
        LevelManager.outerPoints = pointManager.SpawnOuterPoints(LevelManager.innerPoints);

        playerManager.SpawnPlayers(pointManager.radius, ChooseControls.playerStates.Where(o => o.Value != PlayerState.Deactivated).Select(i => i.Key).ToArray());

        playerManager.PlayersLookAtPoint(_inGameManager.levelCenter);
        meshManager.SetMaterials();
        arqdutManager.SpawnArqduts(LevelManager.innerPoints, _inGameManager.levelCenter);

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(LevelManager.innerPoints, LevelManager.outerPoints));
        _inGameManager.DrawMesh(corners);

        if (ChooseControls.gameStarted)
            _gameManager.StartCountdown(_inGameManager.levelCenter);
    }
}