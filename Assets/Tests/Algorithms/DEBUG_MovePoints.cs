using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DEBUG_MovePoints : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private string withTag;

    private Transform[] points;
    private Vector3 center;

    private void Awake()
    {
        this.points = GameObject.FindGameObjectsWithTag(this.withTag).Select(gobj => gobj.transform).ToArray();
    }

    private void OnDrawGizmos()
    {
        if (this.points == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        foreach (var point in this.points)
        {
            Gizmos.DrawLine(point.position, this.GetCenter(this.points.Select(t => t.position)));
        }
    }

    private Vector3 GetCenter(IEnumerable<Vector3> points)
    {
        int count = points.Count();
        return points.Aggregate(Vector3.zero, (acc, next) => acc + next / count);
    }

    private void MovePoints(IList<Vector3> points, Vector3 newCenter)
    {
        var currentCenter = this.GetCenter(points);
        var delta = newCenter - currentCenter;

        for (int i = 0; i < points.Count; i++)
        {
            points[i] += delta;
        }
    }

    private void MovePoints(Vector3[] points, Vector3 newCenter)
    {
        var currentCenter = this.GetCenter(points);
        var delta = newCenter - currentCenter;

        for (int i = 0; i < points.Length; i++)
        {
            points[i] += delta;
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Vector3 pointerPos = eventData.position;
        pointerPos.z = -Camera.main.transform.position.z;
        var newCenter = Camera.main.ScreenToWorldPoint(pointerPos);

        var points = this.points.Select(t => t.position).ToList();
        var prevFirst = points.First();
        this.MovePoints(points, newCenter);
        var currFirst = points.First();

        Debug.Assert(prevFirst != currFirst, "Points weren't moved.");

        int i = 0;
        foreach (var point in points)
        {
            this.points[i++].position = point;
        }
    }
}