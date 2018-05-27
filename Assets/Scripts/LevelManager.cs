using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public delegate void PlayerDestroy(int destroyedPlayerOrder);

    public static event PlayerDestroy OnPlayerDestroy;

    public static List<Vector2> innerPoints = new List<Vector2>(), outerPoints = new List<Vector2>();
    public static List<Vector2> innerLerpFrom = new List<Vector2>(), innerLerpTo = new List<Vector2>(), outerLerpFrom = new List<Vector2>(), outerLerpTo = new List<Vector2>();
    public Vector2 levelCenter;

    public static int playerToDestroy; // TODO make this obsolete

    public float lerpAmount;
    public float lerpedAmount, previousRotation;

    private LevelPoints pointManager;
    private MeshManager meshManager;
    private PointLerp lerpManager;
    private PlayerManager playerManager;
    private ArqdutManager arqdutManager;
    private BallStart ballManager;

    private LevelLerpCircle circleLerpManager;
    private LevelLerp levelLerpManager;
    private LevelSpawner levelSpawner;

    private bool shouldLerpBigger;
    public static bool shouldLerpToCircle, isCircle, shouldLerpSmaller, shouldSetIndices;

    private void Awake()
    {
        lerpManager = GetComponent<PointLerp>();
        pointManager = GetComponent<LevelPoints>();
        meshManager = GetComponent<MeshManager>();
        playerManager = GetComponent<PlayerManager>();
        arqdutManager = GetComponent<ArqdutManager>();
        ballManager = GetComponent<BallStart>();

        levelLerpManager = new LevelLerp(this, lerpManager, pointManager, meshManager, playerManager, arqdutManager, ballManager);
        circleLerpManager = new LevelLerpCircle(this, lerpManager, pointManager, meshManager, playerManager, arqdutManager, ballManager);
        levelSpawner = new LevelSpawner(this, circleLerpManager, pointManager, playerManager, meshManager, arqdutManager, ballManager);
    }

    private void Start()
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

    public void StartLerpCircleSmaller(int playerOrder)
    {
        circleLerpManager.StartLerpToCircle(playerOrder);

        shouldLerpToCircle = true;
        shouldSetIndices = true;
    }

    public void DrawMesh(int playerAmount)
    {
        if (meshManager.mesh.vertexCount > 0)
        {
            meshManager.AddIndicesAndDrawMesh(playerAmount);
        }

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(innerPoints, outerPoints));
    }

    private void FixedUpdate()
    {
        if (shouldLerpSmaller && !shouldLerpToCircle)
        {
            levelLerpManager.LerpLevel();
        }

        if (shouldLerpToCircle)
        {
            circleLerpManager.LerpCircle();
        }
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