using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class RoundSlider : MonoBehaviour
{
    public static int selectedRoundAmount;
    [SerializeField] private int maxRounds;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = maxRounds;
        slider.onValueChanged.AddListener(OnSliderValueChange);
    }

    public void OnSliderValueChange(float roundAmount)
    {
        selectedRoundAmount = (int)roundAmount;
    }
}