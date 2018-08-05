using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class MuteAudioChannel : MonoBehaviour
{
    public delegate void AudioChange();

    public static event AudioChange VolumeChange;

    [SerializeField] private AudioMixer mixer;

    [SerializeField] private string[] channelsToMute;

    private readonly Dictionary<string, float> savedVolumes = new Dictionary<string, float>();

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggle);

        AudioSlider.SaveVolume += SaveVolume;

        foreach (string channel in channelsToMute)
        {
            savedVolumes.Add(channel, 0);
        }
    }

    private void SaveVolume(string channel, float value)
    {
        if (savedVolumes.ContainsKey(channel))
        {
            savedVolumes[channel] = value;
        }
    }

    private void OnToggle(bool value)
    {
        foreach (string channel in channelsToMute)
        {
            mixer.SetFloat(channel, value ? -80 : savedVolumes[channel]);
        }

        VolumeChange?.Invoke();
    }
}