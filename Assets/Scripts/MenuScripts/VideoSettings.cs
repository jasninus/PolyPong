using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class VideoSettings : MonoBehaviour
{
    private readonly int[][] resolutions = { new[] { 1280, 1024 }, new[] { 1366, 768 }, new[] { 1440, 900 }, new[] { 1920, 1080 }, new[] { 2560, 1440 } };

    public void SetResolution(int value)
    {
        Screen.SetResolution(resolutions[value][0], resolutions[value][1], true);
    }

    public void SwitchScreenMode(bool toggleVal)
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, toggleVal);
    }

    private void UsefulStuffThatMightGetUsed()
    {
        Resolution currentResolution = Screen.currentResolution;
        Resolution[] allSupportedResolutions = Screen.resolutions;
    }
}