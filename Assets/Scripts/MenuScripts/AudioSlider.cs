using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private string channelName;

    public delegate void VolumeChange(string channel, float value);

    public static event VolumeChange SaveVolume;

    private Slider slider;

    private bool notSaveVolume;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderChange);

        SetSliderVal();
        MuteAudioChannel.VolumeChange += SetSliderVal;
    }

    private void SetSliderVal()
    {
        float f;
        mixer.GetFloat(channelName, out f);
        notSaveVolume = true;
        slider.value = f;
    }

    private void OnSliderChange(float value)
    {
        mixer.SetFloat(channelName, value);

        if (!notSaveVolume)
        {
            SaveVolume?.Invoke(channelName, value);
        }
        else
        {
            notSaveVolume = false;
        }
    }
}