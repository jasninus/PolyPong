using System.Linq;
using UnityEngine;

public class LevelSpawner
{
    private readonly LevelManager levelManager;
    private readonly LevelLerpCircle circleLerpManager;
    private readonly LevelPoints pointManager;
    private readonly PlayerManager playerManager;
    private readonly MeshManager meshManager;
    private readonly ArqdutManager arqdutManager;
    private readonly GameStart _gameManager;

    public LevelSpawner(LevelManager levelManager, LevelLerpCircle circleLerpManager, LevelPoints pointManager, PlayerManager playerManager, MeshManager meshManager, ArqdutManager arqdutManager, GameStart gameManager)
    {
        this.levelManager = levelManager;
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
        PlayerColors extraPlayerColor = ChooseControls.activatedPlayers.First(i => !i.Value).Key;
        ChooseControls.activatedPlayers[extraPlayerColor] = true;

        SpawnLevel(3);
        LevelManager.playerToDestroy = PlayerManager.players.First(p => p.color == extraPlayerColor).playerOrder;
        levelManager.StartDestroyPlayer();
        levelManager.lerpedAmount = 1 - 2 * levelManager.lerpAmount;
        circleLerpManager.LerpToCircle();

        Object.Destroy(GameObject.FindWithTag("Ball")); // Destroy extra ball

        ChooseControls.activatedPlayers[extraPlayerColor] = false;
    }

    /// <summary>
    /// Calls all methods necessary for spawning the initial level
    /// </summary>
    public void SpawnLevel(int corners)
    {
        LevelManager.innerPoints = pointManager.SpawnInnerPoints(corners, levelManager.levelCenter);
        LevelManager.outerPoints = pointManager.SpawnOuterPoints(LevelManager.innerPoints);

        playerManager.SpawnPlayers(pointManager.radius);
        playerManager.PlayersLookAtPoint(levelManager.levelCenter);
        meshManager.SetMaterials();
        arqdutManager.SpawnArqduts(LevelManager.innerPoints, levelManager.levelCenter);

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(LevelManager.innerPoints, LevelManager.outerPoints));
        levelManager.DrawMesh(corners);
        _gameManager.StartCountdown(levelManager.levelCenter);
    }
}