using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class SFXSounds : MonoBehaviour
{
    public List<AudioClip> sfxList = new List<AudioClip>();
    public AudioSource audioSource;
    public static SFXSounds instance;
    public Slider volumeSlider;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        audioSource = GetComponent<AudioSource>();
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        audioSource.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }
    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxList.Count)
        {
            audioSource.PlayOneShot(sfxList[index]);
        }
    }

    void OnVolumeChanged(float value)
    {
        audioSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
}
