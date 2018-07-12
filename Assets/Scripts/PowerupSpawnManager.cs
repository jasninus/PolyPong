using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LevelManager))]
public class PowerupSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnablePowerups;

    private Vector2 levelCenter;

    [SerializeField] private float spawnRadius, minimumSpawnCooldown, maximumSpawnCooldown;

    private Powerups[] enabledPowerups;
    private List<Powerups> unavailablePowerups = new List<Powerups>();

    private void Start()
    {
        enabledPowerups = PowerupVals.powerupsActivatedState.Where(p => p.Value).Select(p => p.Key).ToArray();

        levelCenter = GetComponent<LevelManager>().levelCenter;

        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(Random.Range(minimumSpawnCooldown, maximumSpawnCooldown));
        SpawnPowerup();
        StartCoroutine(SpawnTimer());
    }

    /*
     * TEST LIST:
     *
     * Increase despawn time to check if limit works
     * 
     */

    private void SpawnPowerup()
    {
        while (true)
        {
            // Stop if no powerups can be spawned
            if (unavailablePowerups.Count == enabledPowerups.Length)
            {
                unavailablePowerups.Clear();
                return;
            }

            // Determine powerup type and spawn location
            Vector2 spawnLocation = Random.insideUnitCircle * spawnRadius + levelCenter; // TODO implement system so that powerups do not spawn on top of one another
            GameObject gameObjToSpawn = unavailablePowerups.Count == 0 ? ChooseRandomPowerupType() : ChooseRandomPowerupType(unavailablePowerups);
            PowerupBase powerupToSpawn = gameObjToSpawn.GetComponent<PowerupBase>();

            // Check if powerup can be spawned in current conditions
            switch (powerupToSpawn.spawnConditions)
            {
                case SpawnConditions.NotInCircle:
                    if (LevelManager.isCircle)
                    {
                        unavailablePowerups.Add(powerupToSpawn.powerupType);
                        continue;
                    }

                    break;
            }

            powerupToSpawn.target = ChooseRandomPowerupTarget(powerupToSpawn);

            // Check for spawnlimit on selected powerup type
            if (GameObject.FindGameObjectsWithTag("Powerup").Count(o => o.GetComponent<PowerupBase>().powerupType == powerupToSpawn.powerupType) < powerupToSpawn.spawnLimit)
            {
                unavailablePowerups.Clear();
                StartCoroutine(TryDestroyPowerup(powerupToSpawn.despawnTime, Instantiate(gameObjToSpawn, spawnLocation, Quaternion.identity)));
            }
            else
            {
                unavailablePowerups.Add(powerupToSpawn.powerupType);
                continue;
            }

            break;
        }
    }

    private IEnumerator TryDestroyPowerup(float despawnTime, GameObject powerupToDestroy)
    {
        yield return new WaitForSeconds(despawnTime);

        // If the powerup has not been picked up it gets destroyed
        if (powerupToDestroy.GetComponent<Collider2D>().enabled)
        {
            Destroy(powerupToDestroy);
        }
    }

    private GameObject ChooseRandomPowerupType()
    {
        Powerups type = enabledPowerups[Random.Range(0, enabledPowerups.Length)];
        return spawnablePowerups.First(p => p.GetComponent<PowerupBase>().powerupType == type);
    }

    private GameObject ChooseRandomPowerupType(List<Powerups> excludedPowerups)
    {
        List<Powerups> test = new List<Powerups>();
        test.AddRange(enabledPowerups.Where(i => !excludedPowerups.Contains(i)));

        Powerups type = enabledPowerups[Random.Range(0, enabledPowerups.Length)];
        return spawnablePowerups.First(p => p.GetComponent<PowerupBase>().powerupType == type);
    }

    /// <summary>
    /// Returns one of the available targets from the specified powerup type
    /// </summary>
    /// <param name="chosenPowerup">The powerup type to choose from</param>
    /// <returns>The chosen target</returns>
    private PowerupTarget ChooseRandomPowerupTarget(PowerupBase chosenPowerup)
    {
        PowerupTarget[] validTargets = chosenPowerup.validTargets;
        return validTargets[Random.Range(0, validTargets.Length)];
    }
}