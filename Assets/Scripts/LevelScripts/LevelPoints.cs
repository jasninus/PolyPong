using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelPoints : MonoBehaviour
{
    public float borderThickness;
    public float radius;

    /// <summary>
    /// Spawns all inner points in a level
    /// </summary>
    /// <param name="cornerAmount">How many points that should be spawned</param>
    /// <param name="center">The center of the spawned points</param>
    /// <returns>The spawned points</returns>
    public List<Vector2> SpawnInnerPoints(int cornerAmount, Vector2 center)
    {
        List<Vector2> spawnedPoints = new List<Vector2>();

        const float radians = 2 * Mathf.PI;

        for (int i = 0; i < cornerAmount; i++)
        {
            spawnedPoints.Add(new Vector2(Mathf.Cos(radians / cornerAmount * i), Mathf.Sin(radians / cornerAmount * i)) * radius);
        }

        //TODO START FROM HERE. There is some problem about the normal game center, it is shifted
        Vector2 currentCenter = GetCenter(spawnedPoints);
        MovePoints(spawnedPoints, currentCenter, center);

        return spawnedPoints;
    }

    /// <summary>
    /// Spawns all inner points in a level
    /// </summary>
    /// <param name="cornerAmount">How many points that should be spawned</param>
    /// <param name="center">The center of the spawned points</param>
    /// <param name="weights">Weights for side length. Has to have length equal to number of corners</param>
    /// <returns>The spawned points</returns>
    public List<Vector2> SpawnInnerPoints(int cornerAmount, Vector2 center, float[] weights)
    {
        float[] normalizedWeights = weights.Select(f => f).ToArray();

        // Get normalized weights
        for (int i = 0; i < weights.Length; i++)
        {
            // The amount of how much each weight should affect all the other weights
            float normalizationAmount = -weights[i] / weights.Length;

            for (int j = 0; j < weights.Length; j++)
            {
                normalizedWeights[j] += normalizationAmount;
            }
        }

        List<Vector2> spawnedPoints = new List<Vector2>();

        const float radians = 2 * Mathf.PI;

        float prevWeights = 0;

        for (int i = 0; i < cornerAmount; i++)
        {
            spawnedPoints.Add(new Vector2(Mathf.Cos((radians / cornerAmount) * i + normalizedWeights[i] + prevWeights),
                                          Mathf.Sin((radians / cornerAmount) * i + normalizedWeights[i] + prevWeights)) * radius);

            prevWeights += normalizedWeights[i];
        }

        return spawnedPoints;
    }

    [SerializeField] private GameObject corner;

    /// <summary>
    /// Spawns GameObjects at specified points. Primarily used for testing
    /// </summary>
    /// <param name="points">The places in which to spawn GameObjects</param>
    /// <returns>The spawned points</returns>
    public GameObject[] VisualizePoints(IEnumerable<Vector2> points)
    {
        GameObject[] newPoints = new GameObject[points.Count()];

        for (int i = 0; i < points.Count(); i++)
        {
            newPoints[i] = Instantiate(corner, points.ElementAt(i), Quaternion.identity);
        }

        return newPoints;
    }

    /// <summary>
    /// Spawns all the outer points of a level with a specified center
    /// </summary>
    /// <param name="innerPoints">The inner points to base the outer points on</param>
    /// <param name="center">The specified center of the points to be spawned</param>
    /// <returns>The spawned points</returns>
    public List<Vector2> GetOuterPoints(IEnumerable<Vector2> innerPoints, Vector2 center)
    {
        List<Vector2> spawnedPoints = new List<Vector2>();

        for (int i = 0; i < innerPoints.Count(); i++)
        {
            Vector2 fromCenter = (innerPoints.ElementAt(i) - center) * (2 + borderThickness);
            spawnedPoints.Add(Vector2.MoveTowards(innerPoints.ElementAt(i), fromCenter + center, borderThickness));
        }

        return spawnedPoints;
    }

    /// <summary>
    /// Spawns all the outer points of a level
    /// </summary>
    /// <param name="innerPoints">The inner points to base the outer points on</param>
    /// <returns>The spawned points</returns>
    public List<Vector2> GetOuterPoints(IEnumerable<Vector2> innerPoints)
    {
        Vector2 center = GetCenter(innerPoints);

        List<Vector2> spawnedPoints = new List<Vector2>();

        for (int i = 0; i < innerPoints.Count(); i++)
        {
            Vector2 fromCenter = (innerPoints.ElementAt(i) - center) * (2 + borderThickness);
            spawnedPoints.Add(Vector2.MoveTowards(innerPoints.ElementAt(i), fromCenter + center, borderThickness));
        }

        return spawnedPoints;
    }

    /// <summary>
    /// Move points to have new center
    /// </summary>
    /// <param name="points">The points to be moved</param>
    /// <param name="currentCenter">The current center</param>
    /// <param name="newCenter">The new center</param>
    public static void MovePoints(IList<Vector2> points, Vector2 currentCenter, Vector2 newCenter)
    {
        Vector2 delta = newCenter - currentCenter;

        for (int i = 0; i < points.Count; i++)
        {
            points[i] += delta;
        }
    }

    public static Vector2 GetCenter(IEnumerable<Vector2> points)
    {
        return points.Aggregate((total, point) => total + point) / points.Count();
    }

    /// <summary>
    /// Finds the center of points with different weights
    /// </summary>
    /// <param name="points">The points to find the center of</param>
    /// <param name="bottomWeightClamp">The lowest weight</param>
    /// <param name="weights">The weights for the points</param>
    /// <returns>The Vector2 center</returns>
    public static Vector2 GetWeightedCenter(IList<Vector2> points, float bottomWeightClamp, float[] weights)
    {
        Vector2 activatedPointsTotal = Vector2.zero;
        float totalPointWeight = 0;

        for (int i = 0; i < points.Count; i++)
        {
            float pointWeight = (1 - weights[i] / bottomWeightClamp);
            Vector2 pointToAdd = points[i] * pointWeight;
            activatedPointsTotal += pointToAdd;
            totalPointWeight += pointWeight;
        }

        return activatedPointsTotal / totalPointWeight;
    }

    /// <summary>
    /// Rotates the given point(s) around a centerpoint by specified degrees
    /// </summary>
    /// <param name="points">The points to be rotated</param>
    /// <param name="degrees">How many degrees around the z-axis the points should be rotated</param>
    public void RotatePoints(IList<Vector2> points, float degrees)
    {
        Vector2 center = GetCenter(points);

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 fromCenter = points[i] - center;

            points[i] = (Vector2)(Quaternion.Euler(0, 0, degrees) * fromCenter) + center;
        }
    }

    /// <summary>
    /// Spawns the points along the triangle shape that is used to lerp to the circle
    /// </summary>
    /// <param name="innerPoints">The inner points to spawn points between</param>
    /// <param name="numberOfPoints">The number of points between each set of points</param>
    /// <returns>The spawned points</returns>
    public List<Vector2> SpawnCircleLerpPoints(Player.LeftRightPoints playerPoints, List<Vector2> innerPoints, Vector2 center, int numberOfPoints)
    {
        // Inner points
        List<Vector2> points = new List<Vector2>(innerPoints);
        points.Remove(playerPoints.left);
        points.Remove(playerPoints.right);

        Vector2 firstPoint = points[0];
        Vector2 betweenPointsL = (playerPoints.left - points[0]) / numberOfPoints;
        Vector2 betweenPointsR = (playerPoints.right - points[0]) / numberOfPoints;

        // Outer points
        Vector2[] cornerPoints =
        {
            playerPoints.left,
            playerPoints.right,
            points[0]
        };
        Vector2[] outerPoints = GetOuterPoints(cornerPoints).ToArray();

        Vector2 outerFrom = outerPoints[2];
        Vector2 outerBetweenL = (outerPoints[0] - outerFrom) / numberOfPoints;
        Vector2 outerBetweenR = (outerPoints[1] - outerFrom) / numberOfPoints;

        for (int i = 1; i <= numberOfPoints; i++)
        {
            points.Add(firstPoint + betweenPointsL * i);
            points.Add(firstPoint + betweenPointsR * i);
        }
        //R Two for-loops for reducing complexity in sorting points
        points.Add(outerFrom);
        for (int i = 1; i <= numberOfPoints; i++)
        {
            points.Add(outerFrom + outerBetweenL * i);
            points.Add(outerFrom + outerBetweenR * i);
        }

        return points;
    }
}