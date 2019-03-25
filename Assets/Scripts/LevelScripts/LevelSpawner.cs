using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelSpawner
{
    private readonly LevelManager levelManager;
    private readonly LevelLerpCircle circleLerpManager;
    private readonly LevelPoints pointManager;
    private readonly PlayerManager playerManager;
    private readonly MeshManager meshManager;
    private readonly ArqdutManager arqdutManager;
    private readonly GameStart gameManager;

    // Calls a base constructor (the one beneath this one)
    public LevelSpawner(LevelManager levelManager, LevelLerpCircle circleLerpManager, LevelPoints pointManager, PlayerManager playerManager, MeshManager meshManager, ArqdutManager arqdutManager, GameStart gameManager) : this(levelManager, pointManager, playerManager, meshManager, arqdutManager)
    {
        this.circleLerpManager = circleLerpManager;
        this.gameManager = gameManager;
    }

    public LevelSpawner(LevelManager levelManager, LevelPoints pointManager, PlayerManager playerManager, MeshManager meshManager, ArqdutManager arqdutManager)
    {
        this.levelManager = levelManager;
        this.pointManager = pointManager;
        this.playerManager = playerManager;
        this.meshManager = meshManager;
        this.arqdutManager = arqdutManager;
    }

    /// <summary>
    /// Spawns the circle immediately
    /// </summary>
    /// <remarks>Actually spawns an extra player and lerps immediately to a circle</remarks>
    public void SpawnCircle()
    {
        InGameManager inGameManager = (InGameManager)levelManager;

        PlayerColor extraPlayerColor = ChooseControls.playerStates.First(i => i.Value == PlayerState.Deactivated).Key;
        ChooseControls.playerStates[extraPlayerColor] = PlayerState.Activated;

        SpawnLevel(3);
        InGameManager.playerToDestroy = PlayerManager.players.First(p => p.Color == extraPlayerColor).playerOrder;
        inGameManager.StartDestroyPlayer();
        inGameManager.lerpedAmount = 1 - 2 * inGameManager.lerpAmount;
        circleLerpManager.LerpToCircle();

        Object.Destroy(GameObject.FindWithTag("Ball")); // Destroy extra ball

        ChooseControls.playerStates[extraPlayerColor] = PlayerState.Deactivated;
    }

    /// <summary>
    /// Calls all methods necessary for spawning the initial level
    /// </summary>
    public void SpawnLevel(int corners)
    {
        LevelManager.innerPoints = pointManager.SpawnInnerPoints(corners, levelManager.levelCenter);
        LevelManager.outerPoints = pointManager.GetOuterPoints(LevelManager.innerPoints);

        // If level is not spawned when game is started, it should spawn all players for the menu
        if (ChooseControls.gameStarted)
        {
            playerManager.SpawnPlayers(pointManager.radius, ChooseControls.playerStates.Where(o => o.Value != PlayerState.Deactivated).Select(i => i.Key).ToArray());
        }
        else
        {
            playerManager.SpawnPlayers(pointManager.radius, (PlayerColor[])Enum.GetValues(typeof(PlayerColor)));
        }

        playerManager.PlayersLookAtPoint(levelManager.levelCenter);
        meshManager.SetMaterials(ChooseControls.playerStates.Where(i => i.Value != PlayerState.Deactivated).Select(item => item.Key).ToArray());
        arqdutManager.SpawnArqduts(LevelManager.innerPoints, levelManager.levelCenter);

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(LevelManager.innerPoints, LevelManager.outerPoints));
        levelManager.DrawMesh(corners);

        if (ChooseControls.gameStarted)
        {
            gameManager.StartCountdown(levelManager.levelCenter);
        }
    }
}