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
        PlayerColors extraPlayerColor = ChooseControls.activatedPlayers.First(i => !i.Value).Key;
        ChooseControls.activatedPlayers[extraPlayerColor] = true;

        SpawnLevel(3);
        InGameManager.playerToDestroy = PlayerManager.players.First(p => p.color == extraPlayerColor).playerOrder;
        _inGameManager.StartDestroyPlayer();
        _inGameManager.lerpedAmount = 1 - 2 * _inGameManager.lerpAmount;
        circleLerpManager.LerpToCircle();

        Object.Destroy(GameObject.FindWithTag("Ball")); // Destroy extra ball

        ChooseControls.activatedPlayers[extraPlayerColor] = false;
    }

    /// <summary>
    /// Calls all methods necessary for spawning the initial level
    /// </summary>
    public void SpawnLevel(int corners)
    {
        InGameManager.innerPoints = pointManager.SpawnInnerPoints(corners, _inGameManager.levelCenter);
        InGameManager.outerPoints = pointManager.SpawnOuterPoints(InGameManager.innerPoints);

        //if (PlayerManager.backupPlayers.Count > 0)
        //{
        //    foreach (Player backupPlayer in PlayerManager.backupPlayers)
        //    {
        //        ChooseControls.activatedPlayers[backupPlayer.color] = true;
        //    }

        //    playerManager.SpawnPlayers(pointManager.radius, PlayerManager.backupPlayers.Select(p => p.color).ToArray());
        //}
        //else
        //{
        playerManager.SpawnPlayers(pointManager.radius, ChooseControls.activatedPlayers.Where(o => o.Value).Select(i => i.Key).ToArray());
        //}

        playerManager.PlayersLookAtPoint(_inGameManager.levelCenter);
        meshManager.SetMaterials();
        arqdutManager.SpawnArqduts(InGameManager.innerPoints, _inGameManager.levelCenter);

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(InGameManager.innerPoints, InGameManager.outerPoints));
        _inGameManager.DrawMesh(corners);

        if (ChooseControls.gameStarted)
            _gameManager.StartCountdown(_inGameManager.levelCenter);
    }
}