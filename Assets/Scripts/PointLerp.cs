using System.Collections.Generic;
using UnityEngine;

public class PointLerp : MonoBehaviour
{
    public List<Vector2> LerpLevelSmaller(List<Vector2> from, List<Vector2> to, float lerpAmount, int[] joiningPoints, int connectingPoint)
    {
        List<Vector2> lerpedPoints = new List<Vector2>();

        for (int i = 0; i < from.Count; i++)
        {
            if (i == joiningPoints[0] || i == joiningPoints[1]) // Joining point
            {
                int j = connectingPoint == to.Count ? 0 : connectingPoint;
                lerpedPoints.Add(Vector2.Lerp(from[i], to[j], lerpAmount));
            }
            else if (i > joiningPoints[1]) // Has passed joining point
            {
                lerpedPoints.Add(Vector2.Lerp(from[i], to[i - 1], lerpAmount));
            }
            else // Before joining point
            {
                lerpedPoints.Add(Vector2.Lerp(from[i], to[i], lerpAmount));
            }
        }

        return lerpedPoints;
    }

    public Vector2[] LerpLevelBigger()
    {
        return null;
    }

    public List<Vector2> LerpToCircle(List<Vector2> from, List<Vector2> to, float lerpAmount)
    {
        //Debug.Log(from.Count + " " + to.Count);
        List<Vector2> lerpedPoints = new List<Vector2>();
        for (int i = 0; i < from.Count; i++)
        {
            if (i == from.Count - 2 || i == from.Count - 1) // Joining points
            {
                lerpedPoints.Add(Vector2.Lerp(from[i], to[0], lerpAmount));
            }
            else if (i % 2 == 0) // Left side of triangle
            {
                lerpedPoints.Add(Vector2.Lerp(from[i], to[Mathf.RoundToInt(to.Count / 2 - 0.5f * i)], lerpAmount));
            }
            else // Right side of triangle
            {
                lerpedPoints.Add(Vector2.Lerp(from[i], to[to.Count / 2 + (i + 1) / 2], lerpAmount));
            }
        }

        return lerpedPoints;
    }
}