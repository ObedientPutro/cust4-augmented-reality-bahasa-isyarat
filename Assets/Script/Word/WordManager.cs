using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WordManager : MonoBehaviour {

    public static ModelTarget CurrentModelTarget { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject arUI;
    [SerializeField] private GameObject scanGuideUI;

    [Space(10)]
    [Header("Buttons References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resetAnimationButton;
    [SerializeField] private Button resetRotationButton;
    
    private void Awake() {
        backButton.onClick.AddListener(BackToMainMenu); 
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        resetAnimationButton.onClick.AddListener(ResetCurrent);
        resetRotationButton.onClick.AddListener(OnResetRotation);
    }

    private void Start() {
        CurrentModelTarget = null;
        HideUI();
    }

    public void ShowUI() {
        arUI.SetActive(true);
        scanGuideUI.SetActive(false);
    }

    public void HideUI() {
        arUI.SetActive(false);
        scanGuideUI.SetActive(true);
    }

    public void SetActiveModelTarget(ModelTarget newTarget) {
        CurrentModelTarget = newTarget;
    }

    public void Play() {
        CurrentModelTarget.TogglePlayPause(false);
    }

    public void Pause() {
        CurrentModelTarget.TogglePlayPause(true);
    }

    public void ResetCurrent() {
        CurrentModelTarget.ResetAnimation();
    }

    public void OnResetRotation() {
        CurrentModelTarget.ResetRotation();
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
