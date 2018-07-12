using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle), typeof(Image))]
public class IndividualPowerupToggle : MonoBehaviour
{
    [SerializeField] private Powerups powerup;

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
        PowerupVals.powerupsActivatedState[powerup] = toggleVal;
        image.color = toggleVal ? Color.white : Color.gray;
    }
}