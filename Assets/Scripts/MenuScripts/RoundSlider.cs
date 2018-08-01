using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class RoundSlider : MonoBehaviour
{
    public static int selectedRoundAmount;
    [SerializeField] private int maxRounds;

    [SerializeField] private Text displayText;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = maxRounds;
        slider.onValueChanged.AddListener(OnSliderValueChange);
    }

    private void OnSliderValueChange(float roundAmount)
    {
        selectedRoundAmount = (int)roundAmount;

        displayText.text = roundAmount + " round" + (roundAmount > 1 ? "s" : "");
    }
}