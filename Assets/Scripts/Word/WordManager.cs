using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WordManager : MonoBehaviour {

    public static ModelTarget CurrentModelTarget { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject arUI;
    [SerializeField] private GameObject scanGuideUI;
    [SerializeField] private GameObject settingsMenu;

    [Space(10)]
    [Header("Buttons References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resetAnimationButton;
    [SerializeField] private Button resetRotationButton;

    private List<ModelTarget> allModelTargets = new List<ModelTarget>();
    private const string BUTTON_BUBBLE = "button_bubble";
    
    private void Awake() {
        settingsButton.onClick.AddListener(ShowSettingsMenu);
        backButton.onClick.AddListener(BackToMainMenu); 
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        resetAnimationButton.onClick.AddListener(ResetCurrent);
        resetRotationButton.onClick.AddListener(OnResetRotation);
    }

    private void Start() {
        SoundManager.Instance.PlayBGM("main_backsound");
        Screen.orientation = ScreenOrientation.Portrait;
        CurrentModelTarget = null;
        HideUI();
        HideSettingsMenu();
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
        if (!allModelTargets.Contains(newTarget)) allModelTargets.Add(newTarget);

        // Set all others inactive
        foreach (var model in allModelTargets) {
            if (model != newTarget) model.gameObject.SetActive(false);
        }

        // Set new one as current and ensure it's active
        CurrentModelTarget = newTarget;
        newTarget.gameObject.SetActive(true);
    }

    public void ResetActiveModelTarget() {
        CurrentModelTarget = null;
        // Reactivate all in case the user scans again
        foreach (var model in allModelTargets) {
            if (model != null) model.gameObject.SetActive(true);
        }
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

    public void ShowSettingsMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        settingsMenu.SetActive(true);
    }

    public void HideSettingsMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        settingsMenu.SetActive(false);
    }

    public void BackToMainMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SceneManager.LoadScene("MainMenu");
    }
}
