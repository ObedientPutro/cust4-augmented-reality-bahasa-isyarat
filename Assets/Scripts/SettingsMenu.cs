using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [Header("Slider Game Object")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private float bgmVolume = 0.5f;
    private float sfxVolume = 0.5f;

    private void Start() {
        (bgmVolume, sfxVolume) = SoundManager.Instance.GetVolume();
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;
    }

    private void SaveVolume() {
        SoundManager.Instance.SetVolume(bgmVolume, sfxVolume);
    }

    public void ChangeBGMVolume() {
        bgmVolume = bgmSlider.value;
        SaveVolume();
    }

    public void ChangeSFXVolume() {
        sfxVolume = sfxSlider.value;
        SaveVolume();
    }

}