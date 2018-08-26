using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle), typeof(Image))]
public class IndividualPowerupToggle : MonoBehaviour
{
    [SerializeField] private Powerups powerup;

    [SerializeField] private PowerupTarget target;

    private Toggle toggle;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(ChangePowerupVal);
    }

    public void ChangePowerupVal(bool toggleVal)
    {
        // TODO disable target instead and if all targets are disabled; disable powerup

        if (!toggleVal)
        {
            PowerupVals.powerupTargets[powerup].Remove(target);

            if (PowerupVals.powerupTargets[powerup].Count < 1)
            {
                PowerupVals.powerupsActivatedState[powerup] = false;
            }
        }
        else
        {
            PowerupVals.powerupTargets[powerup].Add(target);
            PowerupVals.powerupsActivatedState[powerup] = true;
        }

        image.color = toggleVal ? Color.white : Color.gray;
    }
}