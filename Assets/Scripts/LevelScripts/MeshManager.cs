using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    [HideInInspector] public MeshFilter filter;
    [HideInInspector] public Mesh mesh;
    [HideInInspector] public MeshRenderer rend;

    private PolygonCollider2D coll;

    [SerializeField] private Material[] matArray;

    public static readonly Dictionary<PlayerColors, Material> materials = new Dictionary<PlayerColors, Material>();

    public MeshManager()
    {
        Circle = new CircleMesh(this);
    }

    public CircleMesh Circle { get; }

    private void Awake()
    {
        if (materials.Count > 0)
            materials.Clear();

        foreach (PlayerColors item in Enum.GetValues(typeof(PlayerColors)))
        {
            // Add materials
            materials.Add(item, matArray[(int)item]);
        }

        filter = GetComponent<MeshFilter>();
        rend = GetComponent<MeshRenderer>();
        mesh = filter.mesh;
        coll = GetComponent<PolygonCollider2D>();
    }

    /// <summary>
    /// Combine two Vector2 Lists into one Vector3 list
    /// </summary>
    /// <returns>The combined Lists</returns>
    public static List<Vector3> ConcatV2ListsToV3(List<Vector2> list1, List<Vector2> list2)
    {
        List<Vector3> v3List = list1.Select(v => new Vector3(v.x, v.y)).ToList();
        v3List = v3List.Concat(list2.Select(v => new Vector3(v.x, v.y))).ToList();

        return v3List;
    }

    public Material[] SetMaterials()
    {
        rend.materials = ChooseControls.activatedPlayers.Where(i => i.Value).Select(item => materials[item.Key]).ToArray();
        return rend.materials;
    }

    public void SetVertices(List<Vector3> points)
    {
        mesh.SetVertices(points);
    }

    #region Level

    /// <summary>
    /// Needs to have vertices added. Adds all indices and draw submeshes
    /// </summary>
    public void AddIndicesAndDrawMesh(int cornerAmount) // TODO sorting of indices can be done on start. Doesn't have to be done every frame
    {
        mesh.subMeshCount = cornerAmount;
        List<int> indices = new List<int>();

        DrawNormalSquares(indices, cornerAmount);

        AddOddSquare(indices, cornerAmount);
        mesh.SetTriangles(indices, cornerAmount - 1); // Draw odd mesh
    }

    private void DrawNormalSquares(List<int> list, int edges)
    {
        for (int i = 0; i < edges - 1; i++)
        {
            // Inner triangles
            list.Add(i);
            list.Add(i + 1);
            list.Add(i + edges);

            // Outer triangles
            list.Add(i + 1);
            list.Add(i + 1 + edges);
            list.Add(i + edges);

            // Draw mesh
            mesh.SetTriangles(list, i);
            list.Clear();
        }
    }

    public void AddOddSquare(List<int> list, int edges)
    {
        // Odd inner triangle
        list.Add(edges - 1);
        list.Add(0);
        list.Add(edges * 2 - 1);

        // Odd outer triangle
        list.Add(0);
        list.Add(edges);
        list.Add(edges * 2 - 1);
    }

    #endregion Level
}