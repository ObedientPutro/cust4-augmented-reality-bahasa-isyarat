using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AlphabetManager : MonoBehaviour {

    public static ModelTarget CurrentModelTarget { get; private set; }

    [Header("Core References")]
    [SerializeField] private ModelTarget modelTarget;
    [SerializeField] private Image alphabetImage;
    [SerializeField] private Animator animator;
    [SerializeField] private List<AlphabetData> alphabetList;

    [Space(10)]
    [Header("UI References")]
    [SerializeField] private GameObject arUI;
    [SerializeField] private GameObject scanGuideUI;
    [SerializeField] private GameObject settingsMenu;

    [Space(10)]
    [Header("Buttons References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resetAnimationButton;
    [SerializeField] private Button resetRotationButton;

    private int currentIndex = 0;

    private const string BUTTON_BUBBLE = "button_bubble";

    private void Awake() {
        settingsButton.onClick.AddListener(ShowSettingsMenu);
        backButton.onClick.AddListener(BackToMainMenu);
        nextButton.onClick.AddListener(NextSign);
        previousButton.onClick.AddListener(PreviousSign);    
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        resetAnimationButton.onClick.AddListener(ResetCurrent);
        resetRotationButton.onClick.AddListener(OnResetRotation);
    }

    private void Start() {
        SoundManager.Instance.PlayBGM("main_backsound");
        Screen.orientation = ScreenOrientation.Portrait;
        CurrentModelTarget = modelTarget;
        HideUI();
        LoadCurrentSign();
        HideSettingsMenu();
    }

    private void LoadCurrentSign() {
        AlphabetData data = alphabetList[currentIndex];

        // Set sprite
        alphabetImage.sprite = data.letterSprite;

        // Play animation directly
        modelTarget.SetAnimatorController(data.animatorController);
        modelTarget.PlayCurrentAnimation();
    }

    public void ShowUI() {
        arUI.SetActive(true);
        scanGuideUI.SetActive(false);
    }

    public void HideUI() {
        arUI.SetActive(false);
        scanGuideUI.SetActive(true);
    }

    public void NextSign() {
        currentIndex = (currentIndex + 1) % alphabetList.Count;
        LoadCurrentSign();
    }

    public void PreviousSign() {
        currentIndex = (currentIndex - 1 + alphabetList.Count) % alphabetList.Count;
        LoadCurrentSign();
    }

    public void Play() {
        modelTarget.TogglePlayPause(false);
    }

    public void Pause() {
        modelTarget.TogglePlayPause(true);
    }

    public void ResetCurrent() {
        modelTarget.ResetAnimation();
    }

    public void OnResetRotation() {
        modelTarget.ResetRotation();
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
