using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Powerups
{
    Reverse,
    Shield,
    Psychadelic,
    Speed,
    SizeIncrease,
    SizeDecrease,
    Split,
    Invisibility,
    Swallow
}

public class PowerupVals : MonoBehaviour
{
    public static bool shouldSpawnPowerups;

    public static Dictionary<Powerups, bool> powerupsActivatedState = new Dictionary<Powerups, bool>();

    public static Dictionary<Powerups, List<PowerupTarget>> powerupTargets = new Dictionary<Powerups, List<PowerupTarget>>();

    [SerializeField] private GameObject[] powerups;

    private void Awake()
    {
        if (powerupsActivatedState.Count > 0)
            return;

        // Add all targets to PowerupVals
        foreach (GameObject powerupObj in powerups)
        {
            PowerupBase powerup = powerupObj.GetComponent<PowerupBase>();

            powerupTargets.Add(powerup.powerupType, powerup.validTargets.ToList());
        }

        foreach (Powerups item in Enum.GetValues(typeof(Powerups)))
        {
            // All powerups are enabled by default
            powerupsActivatedState.Add(item, true);
        }
    }
}