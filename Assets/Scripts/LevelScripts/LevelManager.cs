using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshManager))]
[RequireComponent(typeof(PlayerManager), typeof(ArqdutManager), typeof(LevelPoints))]
public class LevelManager : MonoBehaviour
{
    [HideInInspector] public float previousRotation;

    public static List<Vector2> innerPoints = new List<Vector2>(), outerPoints = new List<Vector2>();
    public static List<Vector2> innerLerpFrom = new List<Vector2>(), innerLerpTo = new List<Vector2>(), outerLerpFrom = new List<Vector2>(), outerLerpTo = new List<Vector2>();
    public Vector2 levelCenter;

    protected PlayerManager playerManager;
    protected ArqdutManager arqdutManager;
    protected LevelPoints pointManager;
    protected MeshManager meshManager;
    protected LevelSpawner levelSpawner;
    protected LevelLerpCircle circleLerpManager;

    public static bool shouldLerpToCircle, shouldSetIndices;

    protected virtual void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        pointManager = GetComponent<LevelPoints>();
        meshManager = GetComponent<MeshManager>();
        arqdutManager = GetComponent<ArqdutManager>();

        levelSpawner = new LevelSpawner(this, pointManager, playerManager, meshManager, arqdutManager);
    }

    // Used for psychedelic powerup
    public void UpdatePsychedelicLevel(float sinRotMagnitude, float[] weightedRadians, float magnitude, float angle)
    {
        innerPoints = pointManager.SpawnInnerPoints(innerPoints.Count, levelCenter, weightedRadians);
        LevelPoints.MovePoints(innerPoints, LevelPoints.GetCenter(innerPoints), levelCenter);
        pointManager.RotatePoints(innerPoints, -Mathf.Sin(angle) * magnitude * sinRotMagnitude + previousRotation);
        outerPoints = pointManager.GetOuterPoints(innerPoints);

        playerManager.UpdatePlayerPositions();
        playerManager.PlayersLookAtPoint(levelCenter);
        arqdutManager.UpdateArqdutPositions(innerPoints, levelCenter);

        DrawMesh(innerPoints.Count);
    }

    public void DrawMesh(int playerAmount)
    {
        if (meshManager.mesh.vertexCount > 0)
        {
            meshManager.AddIndicesAndDrawMesh(ChooseControls.gameStarted ? playerAmount : 6);
        }

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(innerPoints, outerPoints));
    }

    protected virtual void StartLerpCircleSmaller(int playerOrder)
    {
        circleLerpManager.StartLerpToCircle(playerOrder);

        shouldLerpToCircle = true;
        shouldSetIndices = true;
    }
}