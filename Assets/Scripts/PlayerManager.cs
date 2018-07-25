using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private LevelManager levelManager;

    public static List<Player> players = new List<Player>(), backupPlayers = new List<Player>();

    [SerializeField] private GameObject player;

    [SerializeField] private float playerSpeed, circleSpeed;

    private int leftIndex, rightIndex;

    private Vector2 circleStartLeft, circleStartRight;

    [SerializeField] private CircleCollider[] circleColliders;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    public void SpawnPlayers(float radius)
    {
        var activatedColors = ChooseControls.activatedPlayers.Where(o => o.Value).Select(i => i.Key).ToArray();

        for (int i = 0; i < activatedColors.Length - 1; i++)
        {
            var p = BotSelection.botDifficulties[activatedColors[i]] == 0 ? 
                Instantiate(player, (LevelManager.innerPoints[i] + LevelManager.innerPoints[i + 1]) / 2, Quaternion.identity).AddComponent<Player>() : 
                Instantiate(player, (LevelManager.innerPoints[i] + LevelManager.innerPoints[i + 1]) / 2, Quaternion.identity).AddComponent<Bot>();


            p.Initialize(activatedColors[i], LevelManager.innerPoints[i], LevelManager.innerPoints[i + 1], i, levelManager, playerSpeed, circleSpeed, radius);
            players.Add(p);

            p.transform.GetComponentInChildren<SpriteRenderer>().color = MeshManager.materials[activatedColors[i]].color;
            
            //Debug.Log("Spawned " + activatedColors[i] + " " + p.GetType());
        }

        Player lP = BotSelection.botDifficulties[activatedColors.Last()] == 0 ? 
            Instantiate(player, (LevelManager.innerPoints.Last() + LevelManager.innerPoints.First()) / 2, Quaternion.identity).AddComponent<Player>() : 
            Instantiate(player, (LevelManager.innerPoints.Last() + LevelManager.innerPoints.First()) / 2, Quaternion.identity).AddComponent<Bot>();
            
        lP.Initialize(activatedColors.Last(), LevelManager.innerPoints.Last(), LevelManager.innerPoints.First(), activatedColors.Length - 1, levelManager, playerSpeed, circleSpeed, radius);
        players.Add(lP);

        lP.transform.GetComponentInChildren<SpriteRenderer>().color = MeshManager.materials[activatedColors.Last()].color;


        Debug.Log("Spawned " + activatedColors.Last() + " " + lP.GetType());

        Player[] playerArr = new Player[players.Count];
        players.CopyTo(playerArr);
        backupPlayers = playerArr.ToList();
    }

    public static void V2LookAt(Transform transform, Vector2 point)
    {
        Vector2 v2Position = new Vector2(transform.position.x, transform.position.y);
        Vector2 normPoint = (point + v2Position).normalized;

        float zRotation = Mathf.Atan2(normPoint.y, normPoint.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    public void PlayersLookAtPoint(Vector2 point)
    {
        foreach (var item in players)
        {
            V2LookAt(item.transform, point);
        }
    }

    public void UpdatePlayerPositions()
    {
        foreach (var item in players)
        {
            if (item != players.Last())
            {
                item.points.left = LevelManager.innerPoints[item.playerOrder];
                item.points.right = LevelManager.innerPoints[item.playerOrder + 1];
                item.transform.position = (item.points.left + item.points.right) / 2;
            }
            else
            {
                players.Last().points.left = LevelManager.innerPoints.Last();
                players.Last().points.right = LevelManager.innerPoints.First();
                players.Last().transform.position = (players.Last().points.left + players.Last().points.right) / 2;
            }
        }
    }

    public void CircleSetPlayerPoints(List<Vector2> toPoints, int destroyedPlayer)
    {
        leftIndex = destroyedPlayer + 1 > 2 ? 0 : destroyedPlayer + 1; // Left player
        players[leftIndex].points.left = toPoints[toPoints.Count / 4];
        circleStartLeft = players[leftIndex].transform.position;
        players[leftIndex].SetChildLerpFrom();

        rightIndex = destroyedPlayer - 1 < 0 ? 2 : destroyedPlayer - 1; // Right player
        players[rightIndex].points.left = toPoints[(toPoints.Count / 4) * 3];
        circleStartRight = players[rightIndex].transform.position;
        players[rightIndex].SetChildLerpFrom();
    }

    public void CircleUpdatePlayerPosition(float lerpAmount)
    {
        players[leftIndex].transform.position = Vector3.Lerp(circleStartLeft, players[leftIndex].points.left, lerpAmount);
        players[leftIndex].LerpPlayerPosition(lerpAmount);

        players[rightIndex].transform.position = Vector3.Lerp(circleStartRight, players[rightIndex].points.left, lerpAmount);
        players[rightIndex].LerpPlayerPosition(lerpAmount);
    }

    public void SpawnCircleMovementObjs(Vector2 levelCenter)
    {
        for (int i = 0; i < 2; i++)
        {
            players[i].transform.parent = Instantiate(new GameObject(), levelCenter, Quaternion.Euler(0, 0, 180)).transform;
        }
    }

    public void SpawnCircleColliders(int destroyedPlayer)
    {
        circleColliders[0].CreateCircleColliderLeft(players[destroyedPlayer == 1 ? 1 : 0]);
        circleColliders[1].CreateCircleColliderRight(players[destroyedPlayer == 1 ? 0 : 1]);
    }
}