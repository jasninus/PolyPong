using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class PowerupToggle : MonoBehaviour
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(TogglePowerups);
    }

    public void TogglePowerups(bool toggleVal)
    {
        PowerupVals.shouldSpawnPowerups = toggleVal;
        Debug.Log(PowerupVals.shouldSpawnPowerups);
    }
}