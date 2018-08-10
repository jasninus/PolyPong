/*
        if (Input.GetKeyDown("r"))
        {
            innerPoints.Clear();
            outerPoints.Clear();

            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Corner"))
            {
                Destroy(item);
            }

            innerPoints = levelPointsSpawner.SpawnInnerPoints(cornerAmount, levelCenter).ToList();
            outerPoints = levelPointsSpawner.SpawnOuterPoints(innerPoints).ToList();
            shouldDrawMesh = true;
        }

                        Vector2 currentCorner = transform.position;

        //Initial calculations for spawning of points
        float angularSum = (cornerAmount - 2f) * 180f;
        float cornerAngle = angularSum / cornerAmount;
        float invCornerAngle = 180 - cornerAngle;
        float sideLength = 2 * radius * Mathf.Sin(Mathf.Deg2Rad * (invCornerAngle / 2));
        Vector2 cornerDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * cornerAngle), Mathf.Sin(Mathf.Deg2Rad * cornerAngle));

        Vector2[] spawnedPoints = new Vector2[cornerAmount];
        //Spawning of inner points
        for (int i = 0; i < cornerAmount; i++)
        {
            Vector2 fromCurrentCorner = (Quaternion.Euler(0, 0, (360 / cornerAmount) * i) * cornerDirection) * sideLength;
            currentCorner = fromCurrentCorner + currentCorner;

            spawnedPoints[i] = currentCorner;
        }

        MovePoints(spawnedPoints, center);

        return spawnedPoints;

            public List<Vector2> StartLerpLevelSmaller(List<Vector2> from, List<Vector2> to, float lerpAmount, int[] joiningPoints, int connectingPoint)
    {
        Debug.Log(from.Count);
        List<Vector2> lerpedPoints = new List<Vector2>(from.Count);
        Debug.Log(lerpedPoints.Count);
        Debug.Log(from.Count);

        for (int i = 0; i < from.Count; i++)
        {
            if (i == joiningPoints[0] || i == joiningPoints[1]) // Joining point
            {
                int j = connectingPoint == to.Count ? 0 : connectingPoint;
                Debug.Log(lerpedPoints[i]);
                Debug.Log(from[i]);
                Debug.Log(to[j]);
                lerpedPoints[i] = Vector2.Lerp(from[i], to[j], lerpAmount);
            }
            else if (i > joiningPoints[1]) // Has passed joining point
            {
                lerpedPoints[i] = Vector2.Lerp(from[i], to[i - 1], lerpAmount);
            }
            else // Before joining point
            {
                lerpedPoints[i] = Vector2.Lerp(from[i], to[i], lerpAmount);
            }
        }

        return lerpedPoints;
    }

            //if (points.Last().x < -2.6f || points[points.Count - 2].x < -2.6f)
   //{
   //    Debug.Log(i + " " + points.Last() + " " + points[points.Count - 2]);
   //}

   //foreach (Vector2 item in points)
   //{
   //    foreach (Vector2 point in points)
   //    {
   //        if (item == point)
   //        {
   //            Debug.Log(item + " " + point);
   //        }
   //    }
   //}
}

    private void VisualizePoints(ref GameObject[] objs, IEnumerable<Vector2> points) // For testing
    {
        for (int i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i]);
        }

        objs = pointManager.VisualizePoints(points);

        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].name = "IPoint" + i;
        }
    }

    private void VisualizePoints(IEnumerable<Vector2> points) // For testing
    {
        pointManager.VisualizePoints(points);
    }

    //[SerializeField] private GameObject corner;
    /// <summary>
    /// Spawns GameObjects at specified points
    /// </summary>
    /// <param name="points">The places in which to spawn GameObjects</param>
    /// <returns>The spawned points</returns>
    //public GameObject[] VisualizePoints(IEnumerable<Vector2> points)
    //{
    //    GameObject[] objs = new GameObject[points.Count()];

    //    for (int i = 0; i < points.Count(); i++)
    //    {
    //        objs[i] = Instantiate(corner, points.ElementAt(i), Quaternion.identity);
    //    }

    //    return objs;
    //}
 */

//public static Dictionary<PlayerColors, Bot> bots;

//public class Bot
//{
//    public bool activated;
//    public int difficulty;
//}

//[SerializeField] private PlayerColors color;

//private void AddDictionaryValues()
//{
//foreach (PlayerColors item in Enum.GetValues(typeof(PlayerColors)))
//{
//// Add all text vars to controlTexts
//bots.Add(item, new Bot());
//}
//}

//public void SelectBotDifficulty(int difficulty) // Run this when button is clicked
//{
//bots[color].activated = true;
//bots[color].difficulty = difficulty;
//}

//if (!controlTexts.ContainsKey(item))
//{
//// Add all text vars to controlTexts
//controlTexts.Add(item, new Text[] { controlFields[(int)item * 2], controlFields[(int)item * 2 + 1] });
//}

//if (!selectionYVals.ContainsKey(item))
//{
//// Add all heights for the selectionBall
//selectionYVals.Add(item, squares[(int)item * 2].position.y);
//}

//if (!activatedPlayers.ContainsKey(item))
//{
//// Add all player colors
//activatedPlayers.Add(item, false);
//}

//if (!controls.ContainsKey(item))
//{
//// Add new PlayerControls for all colors
//controls.Add(item, new PlayerControls());
//}