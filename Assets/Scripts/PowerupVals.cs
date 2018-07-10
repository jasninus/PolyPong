using System;
using System.Collections.Generic;
using UnityEngine;

// TODO Add all powerups here
public enum Powerups
{
    Speed,
    SizeIncrease,
    SizeDecrease,
    Reverse
}

public class PowerupVals : MonoBehaviour
{
    public static bool shouldSpawnPowerups;

    public static Dictionary<Powerups, bool> enabledPowerups = new Dictionary<Powerups, bool>();

    private void Awake()
    {
        foreach (Powerups item in Enum.GetValues(typeof(Powerups)))
        {
            // All powerups are enabled by default
            enabledPowerups.Add(item, true);
        }
    }
}