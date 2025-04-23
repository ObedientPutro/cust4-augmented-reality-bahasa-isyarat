using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [Header("Slider Game Object")]
    [SerializeField] private Slider backsoundSlider;
    [SerializeField] private Slider soundEffectSlider;

    public void Start() {
        backsoundSlider.value = SoundManager.Instance.GetBackgroundMusicVolume();
        soundEffectSlider.value = SoundManager.Instance.GetSoundEffectVolume();
    }

    public void SetBackgroundMusicVolume() {
        SoundManager.Instance.SetBackgroundMusicVolume(backsoundSlider.value);
    }

    public void SetSoundEffectVolume() {
        SoundManager.Instance.SetSoundEffectVolume(soundEffectSlider.value);
    }

}