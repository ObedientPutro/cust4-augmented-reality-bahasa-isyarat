using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SentenceManager : MonoBehaviour {
    public static SentenceManager Instance { get; private set; }

    public enum SentenceState { Scanning, Animating };

    [Header("UI References")]
    [SerializeField] private GameObject arUI;
    [SerializeField] private GameObject scanGuideUI;
    [SerializeField] private GameObject previewUI;
    [SerializeField] private TextMeshProUGUI sentencePreviewText;
    [SerializeField] private GameObject settingsMenu;

    [Space(10)]
    [Header("Core References")]
    [SerializeField] private ModelTarget modelTargetPrefab;
    [SerializeField] private Vector3 modelTargetOffset;

    [Space(10)]
    [Header("Buttons References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button rescanButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resetAnimationButton;
    [SerializeField] private Button resetRotationButton;

    private List<CardIdentity> scannedCards = new List<CardIdentity>();
    private SentenceState sentenceState;
    private ModelTarget spawnedModelTarget;
    private ModelTarget currentModelTarget;
    private const string BUTTON_BUBBLE = "button_bubble";

    private void Awake() {
        if (Instance == null && Instance != this) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        
        settingsButton.onClick.AddListener(ShowSettingsMenu);
        startButton.onClick.AddListener(PlaySentenceAnimation);
        rescanButton.onClick.AddListener(ResetSentence);
        backButton.onClick.AddListener(BackToMainMenu);
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        resetAnimationButton.onClick.AddListener(ResetCurrent);
        resetRotationButton.onClick.AddListener(OnResetRotation);
    }

    private void Start() {
        SoundManager.Instance.PlayBGM("main_backsound");
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        ResetSentence();
        settingsMenu.SetActive(false);
    }

    private void Update() {
        if (sentenceState == SentenceState.Scanning) {
            UpdatePreview();
        }
    }

    private void ResetSentence() {
        sentenceState = SentenceState.Scanning;
        scannedCards.Clear();
        sentencePreviewText.text = "";
        startButton.interactable = false;

        if (spawnedModelTarget != null) {
            Destroy(spawnedModelTarget.gameObject);
            spawnedModelTarget = null;
            currentModelTarget = null;
        }

        HideArUI();
        HidePreviewUI();
        ShowScanGuideUI();
    }

    private void UpdatePreview() {
        scannedCards.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        string preview = string.Join(" ", scannedCards.ConvertAll(card => card.word));
        sentencePreviewText.text = preview;

        Debug.Log(scannedCards.Count.ToString());

        if (scannedCards.Count >= 2) {
            HideArUI();
            ShowPreviewUI();
            HideScanGuideUI();
            startButton.interactable = true;
        } else {
            HideArUI();
            HidePreviewUI();
            ShowScanGuideUI();
            startButton.interactable = false;
        }
    }

    private void SetCurrentModelTarget(ModelTarget modelTarget) {
        currentModelTarget = modelTarget;
    }

    private void PlaySentenceAnimation() {
        if (scannedCards.Count < 2) return;
        sentenceState = SentenceState.Animating;

        int centerIndex = Mathf.CeilToInt((scannedCards.Count - 1) / 2);
        CardIdentity centerCard = scannedCards[centerIndex];

        if (spawnedModelTarget != null) Destroy(spawnedModelTarget.gameObject);
        spawnedModelTarget = Instantiate(modelTargetPrefab, centerCard.transform);
        spawnedModelTarget.transform.localPosition = modelTargetOffset;
        spawnedModelTarget.transform.localRotation = Quaternion.identity;

        SetCurrentModelTarget(spawnedModelTarget);
        currentModelTarget.ResetRotation();

        ShowArUI();
        HidePreviewUI();
        HideScanGuideUI();

        StartCoroutine(PlayAnimationsInOrder());
    }

    private IEnumerator<WaitForSeconds> PlayAnimationsInOrder() {
        foreach (RuntimeAnimatorController controller in scannedCards.Select(card => card.animator)) {
            AnimationClip clip = controller.animationClips.Length > 0 ? controller.animationClips[0] : null;

            if (clip != null) {
                currentModelTarget.SetAnimatorController(controller);
                currentModelTarget.PlayCurrentAnimation();
                
                float duration = clip.length / currentModelTarget.GetAnimationSpeed();
                yield return new WaitForSeconds(duration);
            }
        }
    }

    private void ResetAnimationsInOrder() {
        currentModelTarget.StopAndResetAnimation();
        StopCoroutine(PlayAnimationsInOrder());
    }

    public ModelTarget GetCurrentModelTarget() {
        return currentModelTarget;
    }

    public void OnCardFound(CardIdentity card) {
        if (sentenceState != SentenceState.Scanning) return;
        if (scannedCards.Contains(card)) return;
        scannedCards.Add(card);
    }

    public void OnCardLost(CardIdentity card) {
        if (sentenceState != SentenceState.Scanning) return;
        if (!scannedCards.Contains(card)) return;
        scannedCards.Remove(card);
    }

    public void OnImageFound() {
        if (sentenceState != SentenceState.Animating) return;

        ShowArUI();
        HidePreviewUI();
        HideScanGuideUI();
    }

    public void OnImageLost() {
        if (sentenceState != SentenceState.Animating) return;

        ResetSentence();
        HideArUI();
        HidePreviewUI();
        ShowScanGuideUI();
    }

    public void Play() {
        currentModelTarget.TogglePlayPause(false);
    }

    public void Pause() {
        currentModelTarget.TogglePlayPause(true);
    }

    public void ResetCurrent() {
        ResetAnimationsInOrder();
        StartCoroutine(PlayAnimationsInOrder());
    }

    public void OnResetRotation() {
        currentModelTarget.ResetRotation();
    }

    public void ShowScanGuideUI() {
        scanGuideUI.SetActive(true);
    }

    public void HideScanGuideUI() {
        scanGuideUI.SetActive(false);
    }

    public void ShowArUI() {
        arUI.SetActive(true);
    }

    public void HideArUI() {
        arUI.SetActive(false);
    }

    public void ShowPreviewUI() {
        previewUI.gameObject.SetActive(true);
    }

    public void HidePreviewUI() {
        previewUI.gameObject.SetActive(false);
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
