﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(InGameManager))]
public class PowerupSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnablePowerups;

    private Vector2 levelCenter;

    [SerializeField] private float spawnRadius, minimumSpawnCooldown, maximumSpawnCooldown;

    private Powerups[] enabledPowerups;
    private readonly List<Powerups> unavailablePowerups = new List<Powerups>();

    private void Start()
    {
        enabledPowerups = PowerupVals.powerupsActivatedState.Where(p => p.Value).Select(p => p.Key).ToArray();

        levelCenter = GetComponent<InGameManager>().levelCenter;

        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(Random.Range(minimumSpawnCooldown, maximumSpawnCooldown));
        SpawnPowerup();
        StartCoroutine(SpawnTimer());
    }

    private void SpawnPowerup() // TODO this should be broken up into more methods
    {
        while (true)
        {
            // Stop if no powerups can be spawned
            if (unavailablePowerups.Count == enabledPowerups.Length)
            {
                unavailablePowerups.Clear();
                break;
            }

            // Determine powerup type and spawn location
            Vector2 spawnLocation = Random.insideUnitCircle * spawnRadius + levelCenter; // TODO implement system so that powerups do not spawn on top of one another
            GameObject gameObjToSpawn = unavailablePowerups.Count == 0 ? ChooseRandomPowerupType() : ChooseRandomPowerupType(unavailablePowerups);
            PowerupBase powerupToSpawn = gameObjToSpawn.GetComponent<PowerupBase>();

            // Check if powerup can be spawned in current conditions. Continue if condition is not met and add to unavailable
            switch (powerupToSpawn.spawnConditions)
            {
                case SpawnConditions.NotInCircle:
                    if (InGameManager.isCircle || InGameManager.shouldLerpToCircle)
                    {
                        unavailablePowerups.Add(powerupToSpawn.powerupType);
                        continue;
                    }
                    break;

                case SpawnConditions.NeedsMultipleBalls:
                    if (GameObject.FindGameObjectsWithTag("Ball").Length < 2)
                    {
                        unavailablePowerups.Add(powerupToSpawn.powerupType);
                        continue;
                    }
                    break;
            }

            powerupToSpawn.target = ChooseRandomPowerupTarget(powerupToSpawn);

            // Check for spawnlimit on selected powerup type
            if (GameObject.FindGameObjectsWithTag("Powerup").Count(o => o.GetComponent<PowerupBase>()?.powerupType == powerupToSpawn.powerupType) < powerupToSpawn.spawnLimit)
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

    private IEnumerator TryDestroyPowerup(float despawnTime, GameObject powerupToDestroy) // TODO when destroying player, all powerups affecting that player should be destroyed
    {
        yield return new WaitForSeconds(despawnTime);

        if (powerupToDestroy && powerupToDestroy.GetComponent<SpriteRenderer>().enabled)
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
        // TODO this doesn't seem done
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
        // TODO there should be added a reference to ball, so that when lastPlayerHit is null, the player target cannot be used
        List<PowerupTarget> validTargets = PowerupVals.powerupTargets[chosenPowerup.powerupType];
        return validTargets[Random.Range(0, validTargets.Count)];
    }
}