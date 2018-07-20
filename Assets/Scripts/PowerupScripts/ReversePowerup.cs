using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReversePowerup : PowerupBase
{
    private List<Player> enemies;

    private void Awake()
    {
        powerupType = Powerups.Reverse;
    }

    protected override void EnemyActivate(List<Player> enemies)
    {
        this.enemies = enemies;

        foreach (Player enemy in enemies)
        {
            ChooseControls.PlayerControls controls = ChooseControls.controls[enemy.color];
            string leftKey = controls.leftKey;
            controls.leftKey = controls.rightKey;
            controls.rightKey = leftKey;
        }
    }

    protected override void Revert()
    {
        foreach (Player enemy in enemies)
        {
            ChooseControls.PlayerControls controls = ChooseControls.controls[enemy.color];
            string leftKey = controls.leftKey;
            controls.leftKey = controls.rightKey;
            controls.rightKey = leftKey;
        }
    }
}