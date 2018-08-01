using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class DifficultySlider : MonoBehaviour
{
    private Slider slider;

    [SerializeField] private float maxSpeedModification;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = maxSpeedModification * 2;
        slider.value = slider.maxValue / 2;

        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        BallMovement.ballSpeedModifier = value - slider.maxValue / 2;
    }
}