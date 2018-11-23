using System.Collections;
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

    protected virtual void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        pointManager = GetComponent<LevelPoints>();
        meshManager = GetComponent<MeshManager>();
        arqdutManager = GetComponent<ArqdutManager>();
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

    public void DrawMesh(int playerAmount)
    {
        if (meshManager.mesh.vertexCount > 0)
        {
            meshManager.AddIndicesAndDrawMesh(playerAmount);
        }

        meshManager.SetVertices(MeshManager.ConcatV2ListsToV3(innerPoints, outerPoints));
    }
}