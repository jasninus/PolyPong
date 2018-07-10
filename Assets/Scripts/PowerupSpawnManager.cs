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

    private Powerups[] powerups;

    private void Start()
    {
        powerups = PowerupVals.enabledPowerups.Where(p => p.Value).Select(p => p.Key).ToArray();

        levelCenter = GetComponent<LevelManager>().levelCenter;

        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(Random.Range(minimumSpawnCooldown, maximumSpawnCooldown));

        SpawnPowerup();
        StartCoroutine(SpawnTimer());
    }

    private void SpawnPowerup()
    {
        Vector2 spawnLocation = Random.insideUnitCircle * spawnRadius + levelCenter;

        GameObject powerupToSpawn = ChooseRandomPowerupType();

        powerupToSpawn.GetComponent<PowerupBase>().target = ChooseRandomPowerupTarget(powerupToSpawn.GetComponent<PowerupBase>());
    }

    private GameObject ChooseRandomPowerupType()
    {
        Powerups type = powerups[Random.Range(0, powerups.Length)];
        return spawnablePowerups.First(p => p.GetComponent<PowerupBase>().powerupType == type);
    }

    private PowerupTarget ChooseRandomPowerupTarget(PowerupBase chosenPowerup)
    {
        PowerupTarget[] validTargets = chosenPowerup.validTargets;
        return validTargets[Random.Range(0, validTargets.Length)];
    }
}