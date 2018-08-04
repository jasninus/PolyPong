using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class BallMesh : MonoBehaviour
{
    [SerializeField] private float radius;

    /// <summary>
    /// Must be divisible by 6, 5, 4
    /// </summary>
    [SerializeField] private int ballEdges = 60;

    private int currentPlayers;

    private MeshRenderer rend;
    private MeshFilter filter;
    private Mesh mesh;

    [SerializeField] private MeshManager meshManager;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        mesh = filter.mesh;

        meshManager = GameObject.FindWithTag("LevelHandler").GetComponent<MeshManager>();
    }

    private void Start()
    {
        //Material[] mats = new Material[2];

        //if (LevelManager.isCircle)
        //{
        //    for (int i = 0; i < 2; i++)
        //    {
        //        mats[i] = MeshManager.materials[PlayerManager.players[i].color];
        //    }
        //}

        rend.materials = LevelManager.isCircle ? PlayerManager.players.Select(p => MeshManager.materials[p.color]).ToArray() : meshManager.rend.materials;

        currentPlayers = rend.materials.Length;
    }

    private void FixedUpdate()
    {
        AddVertices();
        AddIndices();
    }

    private Vector3[] GeneratePoints()
    {
        Vector3[] spawnedPoints = new Vector3[ballEdges + 1];

        const float radians = 2 * Mathf.PI;

        for (int i = 0; i < ballEdges; i++)
        {
            spawnedPoints[i] = (new Vector2(Mathf.Cos((radians / ballEdges) * i), Mathf.Sin((radians / ballEdges) * i)) * radius);
        }

        return spawnedPoints;
    }

    private void AddVertices()
    {
        mesh.vertices = GeneratePoints();
    }

    private void AddIndices() // TODO this list does not have to be generated each frame
    {
        List<int> indices = new List<int>(); // TODO could be fun to test whether it is most efficient to have this here or in the first for-loop

        mesh.subMeshCount = currentPlayers;

        for (int i = 0; i < currentPlayers; i++) // Code for resetting and getting ready for new adding of tris
        {
            for (int j = 1; j < ballEdges / currentPlayers + 1; j++) // All tris for each side
            {
                if (i == currentPlayers - 1 && j == ballEdges / currentPlayers) // Last odd tri
                {
                    indices.Add(mesh.vertexCount - 1);
                    indices.Add(0);
                    indices.Add(mesh.vertexCount - 2);
                    continue;
                }

                indices.Add(mesh.vertexCount - 1);
                indices.Add((ballEdges / currentPlayers) * i + j);
                indices.Add((ballEdges / currentPlayers) * i - 1 + j);
            }

            mesh.SetTriangles(indices, i);

            indices.Clear();
        }
    }
}