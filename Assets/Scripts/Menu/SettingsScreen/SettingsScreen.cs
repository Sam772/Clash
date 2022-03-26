using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SettingsScreen : MenuScreen {
    public AudioMixer audioMixer;
    [SerializeField] public Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private void Start() {
        fullscreenToggle.isOn = false;
    }

    public void ChangeVolume() {
        AudioListener.volume = volumeSlider.value;
        SaveVolume();
    }

    public void LoadVolume() {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
    }

    private void SaveVolume() {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }  

    public void SetFullScreen(bool isFullScreen) {
        Screen.fullScreen = isFullScreen;
    }
}