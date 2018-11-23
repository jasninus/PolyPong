using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LevelPoints), typeof(MeshManager), typeof(PlayerManager))]
[RequireComponent(typeof(PointLerp), typeof(ArqdutManager))]
public class InGameManager : LevelManager
{
    public delegate void PlayerDestroy(int destroyedPlayerOrder);

    public static event PlayerDestroy OnPlayerDestroy;

    public static int playerToDestroy;

    [HideInInspector] public float lerpedAmount, lerpAmount;

    [SerializeField] private float lerpSmallerModifier, lerpLargerModifier;
    public float baseLerpAmount;

    private PointLerp lerpManager;
    private GameStart _gameManager;
    private DirectionArrows arrowManager;
    private LevelLerpCircle circleLerpManager;
    private LevelLerp levelLerpManager;
    protected LevelSpawner levelSpawner;

    public Player circleSpawningPlayer;

    private bool shouldLerpToNormal;
    public bool shouldLerpFromCircle;
    public static bool shouldLerpToCircle, isCircle, shouldLerpSmaller, shouldSetIndices;

    protected override void Awake()
    {
        base.Awake();

        lerpManager = GetComponent<PointLerp>();
        _gameManager = GetComponent<GameStart>();
        arrowManager = GetComponent<DirectionArrows>();

        // Different classes containing functionality made for better overview
        levelLerpManager = new LevelLerp(this, lerpManager, pointManager, meshManager, playerManager, arqdutManager, _gameManager); // TODO warning about using new keyword. Seems like it thinks I'm trying to add classes as components
        circleLerpManager = new LevelLerpCircle(this, lerpManager, pointManager, meshManager, playerManager, arqdutManager, _gameManager, arrowManager);
        levelSpawner = new LevelSpawner(this, circleLerpManager, pointManager, playerManager, meshManager, arqdutManager, _gameManager);
    }

    private void Start()
    {
        if (ChooseControls.playerStates.Count(i => i.Value != PlayerState.Deactivated) < 3) // Start as circle
        {
            levelSpawner.SpawnCircle();
            return;
        }

        levelSpawner.SpawnLevel(ChooseControls.playerStates.Count(i => i.Value != PlayerState.Deactivated)); // Start normal
    }

    public void StartLerpLevelSmaller(int playerOrder)
    {
        DestroyConditionalPowerups(SpawnConditions.NeedsMultipleBalls);

        levelLerpManager.StartLerpSmaller(playerOrder);

        shouldLerpSmaller = true;
        shouldSetIndices = true;
    }

    private void DestroyConditionalPowerups(SpawnConditions condition)
    {
        GameObject[] toDestroy = GameObject.FindGameObjectsWithTag("Powerup").
            Where(o => o.GetComponent<PowerupBase>().spawnConditions == condition).
            Select(p => p.gameObject).ToArray();

        foreach (GameObject powerup in toDestroy)
        {
            Destroy(powerup);
        }
    }

    public void StartLerpCircleSmaller(int playerOrder)
    {
        DestroyConditionalPowerups(SpawnConditions.NotInCircle);

        circleLerpManager.StartLerpToCircle(playerOrder);

        shouldLerpToCircle = true;
        shouldSetIndices = true;
    }

    public void ReturnToNormalLevel()
    {
        innerLerpFrom = innerPoints;
        outerLerpFrom = outerPoints;

        shouldLerpToNormal = true;
    }

    private void LerpToNormalLevel()
    {
        innerPoints = lerpManager.LerpToNormal(innerLerpFrom, innerLerpTo, lerpedAmount);
        outerPoints = lerpManager.LerpToNormal(outerLerpFrom, outerLerpTo, lerpedAmount);

        DrawMesh(innerPoints.Count);

        playerManager.UpdatePlayerPositions();
        playerManager.PlayersLookAtPoint(levelCenter);
        arqdutManager.UpdateArqdutPositions(innerPoints, levelCenter);
    }

    private void FixedUpdate()
    {
        if (shouldLerpToNormal)
        {
            LerpToNormalLevel();
            UpdateLerpedAmount();
        }

        if (shouldLerpSmaller && !shouldLerpToCircle)
        {
            levelLerpManager.LerpLevel();
        }

        if (shouldLerpToCircle)
        {
            circleLerpManager.LerpCircle();
        }

        if (shouldLerpFromCircle)
        {
            circleLerpManager.LerpFromCircle();
        }
    }

    private void UpdateLerpedAmount()
    {
        lerpedAmount += lerpAmount;
        shouldLerpToNormal = lerpedAmount < 1 + lerpAmount;

        if (!shouldLerpToNormal)
            lerpedAmount = 0;
    }

    public void DestroyPlayer(int playerToDestroy)
    {
        arqdutManager.DestroyArqdut(innerPoints.Count > ChooseControls.playerStates.Count(p => p.Value != PlayerState.Deactivated) ? playerToDestroy == 0 ? 2 : playerToDestroy : playerToDestroy);
        OnPlayerDestroy(playerToDestroy);
        PlayerManager.players.RemoveAt(playerToDestroy);
    }

    public void StartDestroyPlayer()
    {
        if (innerPoints.Count < 4)
        {
            PlayerManager.players[playerToDestroy].CircleDestroyPlayer();
            return;
        }

        PlayerManager.players[playerToDestroy].DestroyPlayer();
    }
}