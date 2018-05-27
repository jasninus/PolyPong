using System.Collections.Generic;
using UnityEngine;

public class CircleMesh : MonoBehaviour
{
    private readonly MeshManager meshManager;

    public CircleMesh(MeshManager meshManager)
    {
        this.meshManager = meshManager;
    }

    public void CircleAddIndicesAndDrawMesh(int edges, int playerOrder)
    {
        meshManager.mesh.subMeshCount = 3;
        List<int> indices = new List<int>();

        // Left side
        CircleAddLeftSquares(edges * 2, indices);
        meshManager.mesh.SetTriangles(indices, playerOrder + 1 > 2 ? 0 : playerOrder + 1);
        indices.Clear();

        // Right side
        CircleAddOddRightSquare(edges * 2, indices);
        CircleAddRightSquares(edges * 2, indices);
        meshManager.mesh.SetTriangles(indices, playerOrder - 1 < 0 ? 2 : playerOrder - 1);
        indices.Clear();

        CircleDrawPlayerToDestroy(edges * 2, playerOrder, indices);
    }

    private void CircleDrawPlayerToDestroy(int twoXEdges, int playerOrder, List<int> indices)
    {
        indices.Add(twoXEdges * 2 + 1);
        indices.Add(twoXEdges * 2);
        indices.Add(twoXEdges);

        indices.Add(twoXEdges * 2);
        indices.Add(twoXEdges - 1);
        indices.Add(twoXEdges);
        meshManager.mesh.SetTriangles(indices, playerOrder);
    }

    private void CircleAddRightSquares(int twoXEdges, List<int> indices)
    {
        for (int i = 2; i < twoXEdges; i += 2) // Right side
        {
            // Inner triangle
            indices.Add(i - 1);
            indices.Add(i + 1);
            indices.Add(i - 1 + twoXEdges + 1);

            // Outer triangle
            indices.Add(i - 1 + twoXEdges + 1);
            indices.Add(i + 1);
            indices.Add(i + 1 + twoXEdges + 1);
        }
    }

    private void CircleAddOddRightSquare(int twoXEdges, List<int> indices)
    {
        indices.Add(0);
        indices.Add(1);
        indices.Add(twoXEdges + 1);

        indices.Add(twoXEdges + 1);
        indices.Add(1);
        indices.Add(twoXEdges + 2);
    }

    private void CircleAddLeftSquares(int twoXEdges, List<int> indices)
    {
        for (int i = 0; i < twoXEdges; i += 2) // Left side
        {
            // Inner triangle
            indices.Add(i + 2);
            indices.Add(i);
            indices.Add(i + twoXEdges + 1);

            // Outer triangle
            indices.Add(i + twoXEdges + 1);
            indices.Add(i + 2 + twoXEdges + 1);
            indices.Add(i + 2);
        }
    }
}