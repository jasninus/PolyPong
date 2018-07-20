using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerup : PowerupBase
{
    private Player affectedPlayer;

    private void Awake()
    {
        powerupType = Powerups.Shield;
    }

    protected override void PlayerActivate(Player lastPlayerHit)
    {
        affectedPlayer = lastPlayerHit;

        if (lastPlayerHit)
            lastPlayerHit.hasShield = true;
    }

    protected override void Revert()
    {
        affectedPlayer.hasShield = false;
    }
}