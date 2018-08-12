using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class ResolutionSettings : MonoBehaviour
{
    private readonly int[][] resolutions = { new[] { 1280, 1024 }, new[] { 1366, 768 }, new[] { 1440, 900 }, new[] { 1920, 1080 }, new[] { 2560, 1440 } };

    public void SetResolution(int value)
    {
        Screen.SetResolution(resolutions[value][0], resolutions[value][1], true);
    }

    private void UsefulStuffThatMightGetUsed()
    {
        Resolution currentResolution = Screen.currentResolution;
        Resolution[] allSupportedResolutions = Screen.resolutions;
    }
}