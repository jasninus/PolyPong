using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class CircleCollider : MonoBehaviour
{
    public delegate void RoundEnd(Player winner);

    public static event RoundEnd RoundWin;

    private Player owner;

    private PolygonCollider2D coll;

    private void Awake()
    {
        coll = GetComponent<PolygonCollider2D>();
    }

    public void CreateCircleColliderRight(Player owner)
    {
        this.owner = owner;

        List<Vector2> vertices = new List<Vector2> { LevelManager.innerPoints[0] };

        for (int i = 1; i < LevelManager.innerPoints.Count; i++)
        {
            if (i % 2 == 1)
            {
                vertices.Add(LevelManager.innerPoints[i]);
            }
        }

        for (int i = 1; i < LevelManager.outerPoints.Count; i++)
        {
            if (i % 2 == 1)
            {
                vertices.Add(LevelManager.outerPoints[LevelManager.outerPoints.Count - (1 + i)]);
            }
        }
        vertices.Add(LevelManager.outerPoints[0]);

        coll.points = vertices.ToArray();
    }

    public void CreateCircleColliderLeft(Player owner)
    {
        this.owner = owner;

        List<Vector2> vertices = LevelManager.innerPoints.Where((t, i) => i % 2 == 0).ToList();

        for (int i = 0; i < LevelManager.outerPoints.Count; i++)
        {
            if (i % 2 == 0)
            {
                vertices.Add(LevelManager.outerPoints[LevelManager.outerPoints.Count - (1 + i)]);
            }
        }

        coll.points = vertices.ToArray();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (owner.hasShield)
        {
            owner.hasShield = false;
            return;
        }

        Destroy(other.gameObject);
        Points.AddPoints(owner.color);
        PlayerManager.players.Remove(owner);
        Points.AddPoints(PlayerManager.players.First().color);
        RoundWin(PlayerManager.players.First());
    }
}