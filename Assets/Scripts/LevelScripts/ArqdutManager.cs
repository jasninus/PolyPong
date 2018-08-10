using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArqdutManager : MonoBehaviour
{
    [SerializeField] private GameObject arqdut;
    private readonly List<GameObject> arqduts = new List<GameObject>();

    public void SpawnArqduts(IEnumerable<Vector2> points, Vector2 center)
    {
        foreach (Vector2 item in points)
        {
            arqduts.Add(Instantiate(arqdut, new Vector3(item.x, item.y, 2.5f), Quaternion.identity));
            PlayerManager.V2LookAt(arqduts.Last().transform, center); // Point arqdut towards center
        }
    }

    public void DestroyAllArqduts()
    {
        foreach (GameObject arqdut in arqduts)
        {
            Destroy(arqdut);
        }

        arqduts.Clear();
    }

    public void DestroyArqdut(int playerOrder)
    {
        Destroy(arqduts[playerOrder]);
        arqduts.RemoveAt(playerOrder);
    }

    public void UpdateArqdutPositions(IList<Vector2> points, Vector2 center)
    {
        for (int i = 0; i < points.Count; i++)
        {
            arqduts[i].transform.position = points[i];
            PlayerManager.V2LookAt(arqduts[i].transform, center);
            arqduts[i].transform.Translate(0, 0, 2.5f);
        }
    }

    public void CircleUpdateArqdutPosition(IList<Vector2> points, Vector2 center)
    {
        arqduts[0].transform.position = points[0];
        arqduts[1].transform.position = points[points.Count - 2];
        try
        {
            arqduts[2].transform.position = points.Last();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        for (int i = 0; i < 3; i++)
        {
            PlayerManager.V2LookAt(arqduts[i].transform, center);
            arqduts[i].transform.Translate(0, 0, 2.5f);
        }
    }
}