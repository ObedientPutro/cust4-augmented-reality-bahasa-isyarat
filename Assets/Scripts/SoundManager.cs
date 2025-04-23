using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance;

    [Space(10)]
    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource buttonSoundEffectSource;

    [Space(10)]
    [Header("Volume Settings")]
    [Range(0, 1)] [SerializeField] private float backgroundMusicDefaultVolume = 1f;
    [Range(0, 1)] [SerializeField] private float soundEffectDefaultVolume = 1f;

    private float backgroundMusicVolume;
    private float soundEffectVolume;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        backgroundMusicVolume = PlayerPrefs.GetFloat("MusicVolume", backgroundMusicDefaultVolume);
        soundEffectVolume = PlayerPrefs.GetFloat("EffectVolume", soundEffectDefaultVolume);

        UpdateBackgroundMusicVolume();
        UpdateSoundEffectVolume();

        PlayBackgroundMusic();
    }

    private void UpdateBackgroundMusicVolume() {
        backgroundMusicSource.volume = backgroundMusicVolume;
    }

    private void UpdateSoundEffectVolume() {
        buttonSoundEffectSource.volume = soundEffectVolume;
    }

    public void PlayBackgroundMusic() {
        backgroundMusicSource.Play();
        backgroundMusicSource.loop = true;
    }

    public void PlayButtonSoundEffect() {
        buttonSoundEffectSource.Play();
    }


    public float GetBackgroundMusicVolume() {
        return backgroundMusicVolume;
    }

    public float GetSoundEffectVolume() {
        return soundEffectVolume;
    }

    public void SetBackgroundMusicVolume(float volume) {
        backgroundMusicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", backgroundMusicVolume);
        UpdateBackgroundMusicVolume();
    }

    public void SetSoundEffectVolume(float volume) {
        soundEffectVolume = volume;
        PlayerPrefs.SetFloat("EffectVolume", soundEffectVolume);
        UpdateSoundEffectVolume();
    }

}
