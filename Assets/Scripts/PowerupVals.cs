﻿using System;
using System.Collections.Generic;
using UnityEngine;

// TODO Add all powerups here
public enum Powerups
{
    Reverse,
    Shield
}

public class PowerupVals : MonoBehaviour
{
    public static bool shouldSpawnPowerups;

    public static Dictionary<Powerups, bool> powerupsActivatedState = new Dictionary<Powerups, bool>();

    private void Awake()
    {
        foreach (Powerups item in Enum.GetValues(typeof(Powerups)))
        {
            // All powerups are enabled by default
            powerupsActivatedState.Add(item, true);
        }
    }
}