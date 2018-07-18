using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerup : PowerupBase
{
    private void Awake()
    {
        powerupType = Powerups.Shield;
    }

    protected override void PlayerActivate(Player lastPlayerHit)
    {
        lastPlayerHit.hasShield = true;
    }
}