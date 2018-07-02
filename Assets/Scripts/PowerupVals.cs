using System;
using System.Collections.Generic;
using UnityEngine;

// Add all powerups here
public enum Powerups
{
    speed,
    sizeIncrease,
    sizeDecrease
}

public class PowerupVals : MonoBehaviour
{
    public static bool shouldSpawnPowerups;

    public static Dictionary<Powerups, bool> enabledPowerups = new Dictionary<Powerups, bool>();

    private void Awake()
    {
        foreach (Powerups item in Enum.GetValues(typeof(Powerups)))
        {
            enabledPowerups.Add(item, false);
        }
    }
}