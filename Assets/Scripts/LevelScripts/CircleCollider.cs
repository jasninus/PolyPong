using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class CircleCollider : MonoBehaviour
{
    public delegate void RoundEnd();

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

        List<Vector2> vertices = new List<Vector2> { InGameManager.innerPoints[0] };

        for (int i = 1; i < InGameManager.innerPoints.Count; i++)
        {
            if (i % 2 == 1)
            {
                vertices.Add(InGameManager.innerPoints[i]);
            }
        }

        for (int i = 1; i < InGameManager.outerPoints.Count; i++)
        {
            if (i % 2 == 1)
            {
                vertices.Add(InGameManager.outerPoints[InGameManager.outerPoints.Count - (1 + i)]);
            }
        }
        vertices.Add(InGameManager.outerPoints[0]);

        coll.points = vertices.ToArray();
    }

    public void CreateCircleColliderLeft(Player owner)
    {
        this.owner = owner;

        List<Vector2> vertices = InGameManager.innerPoints.Where((t, i) => i % 2 == 0).ToList();

        for (int i = 0; i < InGameManager.outerPoints.Count; i++)
        {
            if (i % 2 == 0)
            {
                vertices.Add(InGameManager.outerPoints[InGameManager.outerPoints.Count - (1 + i)]);
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
        Points.AddPoints(owner.Color);
        PlayerManager.players.Remove(owner);
        Points.AddPoints(PlayerManager.players.First().Color);
        RoundWin?.Invoke();
    }
}