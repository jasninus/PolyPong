using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LevelPoints), typeof(MeshManager), typeof(PlayerManager))]
[RequireComponent(typeof(PointLerp), typeof(ArqdutManager))]
public class LevelManager : MonoBehaviour
{
    public delegate void PlayerDestroy(int destroyedPlayerOrder);

    public static event PlayerDestroy OnPlayerDestroy;

    public static List<Vector2> innerPoints = new List<Vector2>(), outerPoints = new List<Vector2>();
    public static List<Vector2> innerLerpFrom = new List<Vector2>(), innerLerpTo = new List<Vector2>(), outerLerpFrom = new List<Vector2>(), outerLerpTo = new List<Vector2>();
    public Vector2 levelCenter;

    public static int playerToDestroy;

    [HideInInspector] public float lerpedAmount, previousRotation, lerpAmount;

    [SerializeField] protected float lerpSmallerModifier, lerpLargerModifier;
    public float baseLerpAmount;

    protected LevelPoints pointManager;
    protected MeshManager meshManager;
    protected PointLerp lerpManager;
    protected PlayerManager playerManager;
    protected ArqdutManager arqdutManager;
    protected GameStart _gameManager;
    protected DirectionArrows arrowManager;

    protected LevelLerpCircle circleLerpManager;
    protected LevelLerp levelLerpManager;
    protected LevelSpawner levelSpawner;

    public Player circleSpawningPlayer;

    protected bool shouldLerpToNormal;
    public bool shouldLerpFromCircle;
    public static bool shouldLerpToCircle, isCircle, shouldLerpSmaller, shouldSetIndices;

    protected virtual void Awake()
    {
        lerpManager = GetComponent<PointLerp>();
        pointManager = GetComponent<LevelPoints>();
        meshManager = GetComponent<MeshManager>();
        playerManager = GetComponent<PlayerManager>();
        arqdutManager = GetComponent<ArqdutManager>();
        _gameManager = GetComponent<GameStart>();
        arrowManager = GetComponent<DirectionArrows>();

        // Different classes containing functionality made for better overview
        levelLerpManager = new LevelLerp(this, lerpManager, pointManager, meshManager, playerManager, arqdutManager, _gameManager); // TODO warning about using new keyword. Seems like it thinks I'm trying to add classes as components
        circleLerpManager = new LevelLerpCircle(this, lerpManager, pointManager, meshManager, playerManager, arqdutManager, _gameManager, arrowManager);
        levelSpawner = new LevelSpawner(this, circleLerpManager, pointManager, playerManager, meshManager, arqdutManager, _gameManager);
    }

    protected virtual void Start()
    {
        if (ChooseControls.activatedPlayers.Count(i => i.Value) < 3) // Start as circle
        {
            levelSpawner.SpawnCircle();
            return;
        }

        levelSpawner.SpawnLevel(ChooseControls.activatedPlayers.Count(i => i.Value)); // Start normal
    }

    public void StartLerpLevelSmaller(int playerOrder)
    {
        levelLerpManager.StartLerpSmaller(playerOrder);

        shouldLerpSmaller = true;
        shouldSetIndices = true;
    }

    private void DestroyNonCirclePowerups()
    {
        PowerupBase[] powerups = GameObject.FindGameObjectsWithTag("Powerup").Select(o => o.GetComponent<PowerupBase>()).ToArray();

        foreach (PowerupBase powerup in powerups.Where(p => p.spawnConditions == SpawnConditions.NotInCircle))
        {
            Destroy(powerup.gameObject);
        }
    }

    public void StartLerpCircleSmaller(int playerOrder)
    {
        DestroyNonCirclePowerups();

        circleLerpManager.StartLerpToCircle(playerOrder);

        shouldLerpToCircle = true;
        shouldSetIndices = true;
    }

    public void UpdatePsychadelicLevel(float sinRotMagnitude, float[] weightedRadians, float magnitude, float angle)
    {
        innerPoints = pointManager.SpawnInnerPoints(innerPoints.Count, levelCenter, weightedRadians);
        pointManager.RotatePoints(innerPoints, -Mathf.Sin(angle) * magnitude * sinRotMagnitude + previousRotation);
        outerPoints = pointManager.SpawnOuterPoints(innerPoints);

        playerManager.UpdatePlayerPositions();
        playerManager.PlayersLookAtPoint(levelCenter);
        arqdutManager.UpdateArqdutPositions(innerPoints, levelCenter);

        DrawMesh(innerPoints.Count);
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

    public void DrawMesh(int playerAmount)
    {
        if (meshManager.mesh.vertexCount > 0)
        {
            meshManager.AddIndicesAndDrawMesh(playerAmount);
        }

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(innerPoints, outerPoints));
    }

    protected virtual void FixedUpdate()
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
        arqdutManager.DestroyArqdut(innerPoints.Count > ChooseControls.activatedPlayers.Count(p => p.Value) ? playerToDestroy == 0 ? 2 : playerToDestroy : playerToDestroy);
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