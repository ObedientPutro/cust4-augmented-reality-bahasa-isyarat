using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WordManager : MonoBehaviour {
    public static WordManager Instance { get; private set; }

    [SerializeField] private List<ModelTarget> modelTargets = new List<ModelTarget>();

    [Space(10)]
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

    private ModelTarget currentModelTarget;

    private const string BUTTON_BUBBLE = "button_bubble";
    
    private void Awake() {
        if (Instance == null && Instance != this) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

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
        currentModelTarget = null;
        HideUI();
        HideSettingsMenu();
    }

    public ModelTarget GetCurrentModelTarget() {
        return currentModelTarget;
    }

    public void ShowUI() {
        arUI.SetActive(true);
        scanGuideUI.SetActive(false);
    }

    public void HideUI() {
        arUI.SetActive(false);
        scanGuideUI.SetActive(true);
    }

    public void SetActiveModelTarget(ModelTarget modelTarget) {
        if (currentModelTarget != null) return;

        // Set new one as current and ensure it's active
        currentModelTarget = modelTarget;
        modelTarget.gameObject.SetActive(true);
        modelTarget.transform.parent?.gameObject.SetActive(true);

        // Show the ui and play it's animation
        ShowUI();
        modelTarget.PlayCurrentAnimation();

        // Set all others inactive, including their parents
        foreach (var model in modelTargets) {
            if (model != currentModelTarget) {
                model.gameObject.SetActive(false);
                model.transform.parent?.gameObject.SetActive(false);
            }
        }
    }

    public void ResetActiveModelTarget(ModelTarget modelTarget) {
        if (currentModelTarget != modelTarget) return;

        // Hide ui and reset the animation
        HideUI();
        currentModelTarget.StopAndResetAnimation();

        currentModelTarget = null;

        // Reactivate all in case the user scans again
        foreach (var model in modelTargets) {
            model.transform.parent?.gameObject.SetActive(true);
            model.gameObject.SetActive(true);
        }
    }

    public void Play() {
        currentModelTarget.TogglePlayPause(false);
    }

    public void Pause() {
        currentModelTarget.TogglePlayPause(true);
    }

    public void ResetCurrent() {
        currentModelTarget.ResetAnimation();
    }

    public void OnResetRotation() {
        currentModelTarget.ResetRotation();
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
