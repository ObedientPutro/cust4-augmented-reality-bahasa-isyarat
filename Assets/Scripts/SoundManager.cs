using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSourcePrefab;

    [Space(5)]
    [Header("Audio Clips")]
    public List<Sound> sounds;

    private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();
    private List<AudioSource> sfxPool = new List<AudioSource>();

    private float bgmVolume = 0.5f;
    private float sfxVolume = 0.5f;

    private const string PREF_BGM_VOLUME = "BGM_VOLUME";
    private const string PREF_SFX_VOLUME = "SFX_VOLUME";

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build sound dictionary for quick lookups
        foreach (var sound in sounds) {
            if (soundDictionary.ContainsKey(sound.soundName.ToLower())) {
                Debug.LogWarning($"Duplicate sound name found: {sound.soundName}");
                continue;
            }

            soundDictionary[sound.soundName.ToLower()] = sound.audioClip;
        }

        // Initialize the SFX pool
        for (int i = 0; i < 5; i++) {
            AudioSource newSource = Instantiate(sfxSourcePrefab, transform);
            newSource.gameObject.SetActive(false);
            sfxPool.Add(newSource);
        }

        LoadVolumeSettings();
    }

    private void LoadVolumeSettings() {
        // Load values from PlayerPrefs, fallback to default if not found
        bgmVolume = PlayerPrefs.GetFloat(PREF_BGM_VOLUME, 0.5f);
        sfxVolume = PlayerPrefs.GetFloat(PREF_SFX_VOLUME, 0.5f);

        ApplyVolumeSettings();
    }

    private void SaveVolumeSettings() {
        PlayerPrefs.SetFloat(PREF_BGM_VOLUME, bgmVolume);
        PlayerPrefs.SetFloat(PREF_SFX_VOLUME, sfxVolume);
        PlayerPrefs.Save();
    }

    private void ApplyVolumeSettings() {
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;

        foreach (var source in sfxPool)
        {
            if (source != null)
                source.volume = sfxVolume;
        }
    }

    private AudioSource GetAvailableAudioSource() {
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying)
            {
                source.gameObject.SetActive(true);
                return source;
            }
        }

        AudioSource newSource = Instantiate(sfxSourcePrefab, transform);
        sfxPool.Add(newSource);
        return newSource;
    }

    // 🔊 Play Sound Effect
    public void PlaySFX(string soundName) {
        if (soundDictionary.TryGetValue(soundName.ToLower(), out AudioClip clip))
        {
            AudioSource source = GetAvailableAudioSource();
            source.clip = clip;
            source.volume = sfxVolume;
            source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
        }
    }

    // 🔊 Play Sound Effect by Clip Parameters
    public void PlaySFXUsingClip(AudioClip audioClip) {
        AudioSource source = GetAvailableAudioSource();
        source.clip = audioClip;
        source.volume = sfxVolume;
        source.Play();
    }

    // 🔊 Play Background Music
    public void PlayBGM(string soundName) {
        if (soundDictionary.TryGetValue(soundName.ToLower(), out AudioClip clip))
        {
            if (bgmSource.clip == clip)
                return;

            StopBGM();

            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{soundName}' not found!");
        }
    }

    // 🔇 Stop Background Music
    public void StopBGM() {
        if (bgmSource != null)
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }
    }

    // 🎚 Change and Save Volume
    public void SetVolume(float newBgmVolume, float newSfxVolume) {
        bgmVolume = Mathf.Clamp01(newBgmVolume);
        sfxVolume = Mathf.Clamp01(newSfxVolume);

        ApplyVolumeSettings();
        SaveVolumeSettings();
    }

    // 🎚 Get Current Volume (Optional: Expose for UI sliders)
    public (float bgm, float sfx) GetVolume() {
        return (bgmVolume, sfxVolume);
    }

    public AudioClip GetAudioClip(string soundName) {
        if (soundDictionary.TryGetValue(soundName.ToLower(), out AudioClip clip)) {
            return clip;
        }

        Debug.LogWarning($"AudioClip '{soundName}' not found!");
        return null;
    }
}
