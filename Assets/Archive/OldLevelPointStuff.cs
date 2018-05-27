using UnityEngine;
/// <summary>
/// Deprecated spawning for level points
/// </summary>
/// <remarks>By Jakob Asgaard</remarks>

public class OldLevelPointStuff : MonoBehaviour
{
  /* void CalculateCorners()
    {
        angularSum = (corners - 2) * 180;
        cornerAngle = angularSum / corners;
        float angle = 180 - cornerAngle;
        sideLength = 2 * radius * Mathf.Sin(Mathf.Deg2Rad * (angle / 2));
        currentCorner = transform.position;
        Vector2 cornerDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * cornerAngle), Mathf.Sin(Mathf.Deg2Rad * cornerAngle));

        for (int i = 0; i < corners; i++)
        {
            float rotationAngle = ((360 / corners) * (i));
            Vector2 fromCurrentCorner = (Quaternion.Euler(0, 0, rotationAngle) * cornerDirection) * sideLength;
            currentCorner = fromCurrentCorner + currentCorner;

            Instantiate(cornerObject, currentCorner, transform.rotation);
        }
    }

    void CalculateBorders()
    {
        savedRadius = radius;
        LevelMeshOBSOLETE.TransformList.Clear();
        outerCirclePoints.Clear();
        combinedOuterVector = new Vector2(0, 0);

        currentCorner = transform.position;
        angularSum = (corners - 2) * 180;
        cornerAngle = angularSum / corners;
        float angle = 180 - cornerAngle;
        sideLength = 2 * radius * Mathf.Sin(Mathf.Deg2Rad * (angle / 2));

        Vector2 cornerDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * cornerAngle), Mathf.Sin(Mathf.Deg2Rad * cornerAngle));

        //Outer points
        for (int i = 0; i < corners; i++)
        {
            rotationAngle = ((360 / corners) * (i));

            if (i == 0)
            {
                sideLength = 0;
            }
            else
            {
                sideLength = 2 * radius * Mathf.Sin(Mathf.Deg2Rad * (angle / 2));
            }

            Vector2 fromCurrentCorner = (Quaternion.Euler(0, 0, rotationAngle) * cornerDirection) * sideLength;
            currentCorner = fromCurrentCorner + currentCorner;

            GameObject g = Instantiate(cornerObject, currentCorner, transform.rotation);
            //Debug.Log(g.transform.position);
            //Debug.Log("Adding transform1");
            g.name = "Outerpoint" + i;
            spawnedPoints.Add(g);
            LevelMeshOBSOLETE.TransformList.Add(g.transform);
            outerCirclePoints.Add(g.transform);
        }

        foreach (Transform item in outerCirclePoints)
        {
            combinedOuterVector += item.position;
        }

        middle = combinedOuterVector / outerCirclePoints.Count;

        currentCorner = Vector2.MoveTowards(currentCorner, middle, borderThickness);
        radius = radius - (borderThickness);
        angularSum = (corners - 2) * 180;
        cornerAngle = angularSum / corners;
        angle = 180 - cornerAngle;
        sideLength = 2 * radius * Mathf.Sin(Mathf.Deg2Rad * (angle / 2));

        cornerDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * cornerAngle), Mathf.Sin(Mathf.Deg2Rad * cornerAngle));

        //Inner points
        for (int i = 0; i < corners; i++)
        {
            rotationAngle = ((360 / corners) * (i));
            //Debug.Log(rotationAngle);
            Vector2 fromCurrentCorner = (Quaternion.Euler(0, 0, rotationAngle) * cornerDirection) * sideLength;
            currentCorner = fromCurrentCorner + currentCorner;

            GameObject g = Instantiate(cornerObject, currentCorner, transform.rotation);
            g.name = "InnerPoint" + i;
            spawnedPoints.Add(g);
            LevelMeshOBSOLETE.TransformList.Add(g.transform);

            SpawnPlayers.innerPoints.Add(g);
            //GetComponent<SpawnPlayers>().spawnPlayers();
        }

        middle = combinedOuterVector / outerCirclePoints.Count;

        if (middle != new Vector2(0, 0))
        {
            Vector2 midDiff = firstMiddle - middle;

            foreach (GameObject item in spawnedPoints)
            {
                item.transform.position = new Vector2(item.transform.position.x - middle.x, item.transform.position.y - middle.y);
            }
        }

        combinedOuterVector = new Vector3(0, 0, 0);
        foreach (Transform item in outerCirclePoints)
        {
            combinedOuterVector += item.position;
        }

        middle = combinedOuterVector / outerCirclePoints.Count;

        //Debug.Log(combinedOuterVector / outerCirclePoints.Count);
        GetComponent<LevelMeshOBSOLETE>().AddPointsAndSubmeshes();
        radius = savedRadius;
        spawnedPoints.Clear();
    }*/
}
