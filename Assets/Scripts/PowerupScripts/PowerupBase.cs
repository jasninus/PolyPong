using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupTarget
{
    Enemies,
    Player,
    Ball,
    Special
}

public enum SpawnConditions
{
    None,
    NotInCircle
}

public abstract class PowerupBase : MonoBehaviour
{
    [HideInInspector] public PowerupTarget target;

    public Powerups powerupType;

    public SpawnConditions spawnConditions;

    public PowerupTarget[] validTargets;

    public int spawnLimit;

    public float despawnTime, duration;

    protected virtual void EnemyActivate(List<Player> enemies)
    {
    }

    protected virtual void PlayerActivate(Player lastPlayerHit)
    {
    }

    protected virtual void BallActivate(GameObject ball)
    {
    }

    protected virtual void SpecialActivate()
    {
    }

    protected virtual void Revert()
    {
    }

    private List<Player> GetAllEnemies(Player lastPlayerHit)
    {
        List<Player> enemies = new List<Player>();

        foreach (Player player in PlayerManager.players)
        {
            if (player != lastPlayerHit)
            {
                enemies.Add(player);
            }
        }

        return enemies;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ball")
        {
            switch (target)
            {
                case PowerupTarget.Enemies:
                    EnemyActivate(GetAllEnemies(other.GetComponent<BallMovement>().lastPlayerHit));
                    break;

                case PowerupTarget.Player:
                    PlayerActivate(other.GetComponent<BallMovement>().lastPlayerHit);
                    break;

                case PowerupTarget.Ball:
                    BallActivate(other.gameObject);
                    break;

                case PowerupTarget.Special:
                    SpecialActivate();
                    break;
            }

            Invoke("Revert", duration);
            Destroy(gameObject, duration + 0.1f);

            // Powerup seems destroyed, but is still able to call Revert()
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}