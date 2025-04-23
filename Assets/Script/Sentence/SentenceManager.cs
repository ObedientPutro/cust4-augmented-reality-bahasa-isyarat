using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SentenceManager : MonoBehaviour {
    public static SentenceManager Instance { get; private set; }

    public static ModelTarget CurrentModelTarget { get; private set; }

    public enum SentenceState { Scanning, Animating };

    [Header("UI References")]
    [SerializeField] private GameObject arUI;
    [SerializeField] private GameObject scanGuideUI;
    [SerializeField] private GameObject previewUI;
    [SerializeField] private TextMeshProUGUI sentencePreviewText;

    [Space(10)]
    [Header("Core References")]
    [SerializeField] private ModelTarget modelTargetPrefab;
    [SerializeField] private Vector3 modelTargetOffset;

    [Space(10)]
    [Header("Buttons References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button rescanButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resetAnimationButton;
    [SerializeField] private Button resetRotationButton;

    private List<CardIdentity> scannedCards = new List<CardIdentity>();
    private SentenceState sentenceState;
    private ModelTarget spawnedModelTarget;

    private void Awake() {
        if (Instance == null && Instance != this) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        startButton.onClick.AddListener(PlaySentenceAnimation);
        rescanButton.onClick.AddListener(ResetSentence);
        backButton.onClick.AddListener(BackToMainMenu);
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        resetAnimationButton.onClick.AddListener(ResetCurrent);
        resetRotationButton.onClick.AddListener(OnResetRotation);
    }

    private void Start() {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        ResetSentence();
    }

    private void ResetSentence() {
        sentenceState = SentenceState.Scanning;
        scannedCards.Clear();
        sentencePreviewText.text = "";
        startButton.interactable = false;

        if (spawnedModelTarget != null) {
            Destroy(spawnedModelTarget.gameObject);
            spawnedModelTarget = null;
            CurrentModelTarget = null;
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
        CurrentModelTarget = modelTarget;
    }

    private void PlaySentenceAnimation() {
        if (scannedCards.Count < 2) return;
        sentenceState = SentenceState.Animating;

        int centerIndex = Mathf.CeilToInt(scannedCards.Count / 2);
        CardIdentity centerCard = scannedCards[centerIndex];

        if (spawnedModelTarget != null) Destroy(spawnedModelTarget.gameObject);
        spawnedModelTarget = Instantiate(modelTargetPrefab, centerCard.transform);
        spawnedModelTarget.transform.localPosition = modelTargetOffset;
        spawnedModelTarget.transform.localRotation = Quaternion.identity;

        SetCurrentModelTarget(spawnedModelTarget);
        CurrentModelTarget.ResetRotation();

        ShowArUI();
        HidePreviewUI();
        HideScanGuideUI();

        StartCoroutine(PlayAnimationsInOrder());
    }

    private IEnumerator<WaitForSeconds> PlayAnimationsInOrder() {
        foreach (var card in scannedCards) {
            RuntimeAnimatorController controller = card.animator;
            AnimationClip clip = controller.animationClips.Length > 0 ? controller.animationClips[0] : null;

            if (clip != null) {
                CurrentModelTarget.SetAnimatorController(controller);
                CurrentModelTarget.PlayCurrentAnimation();
                
                float duration = clip.length / CurrentModelTarget.GetAnimationSpeed();
                yield return new WaitForSeconds(duration);
            }
        }
    }

    private void ResetAnimationsInOrder() {
        CurrentModelTarget.StopAndResetAnimation();
        StopCoroutine(PlayAnimationsInOrder());
    }

    public void OnCardFound(CardIdentity card) {
        if (sentenceState != SentenceState.Scanning) return;

        if (!scannedCards.Contains(card)) {
            scannedCards.Add(card);
            UpdatePreview();
        }
    }

    public void OnCardLost(CardIdentity card) {
        if (sentenceState != SentenceState.Scanning) return;

        if (scannedCards.Contains(card)) {
            scannedCards.Remove(card);
            UpdatePreview();
        }
    }

    public void OnImageFound() {
        if (sentenceState != SentenceState.Animating) return;

        ShowArUI();
        HidePreviewUI();
        HideScanGuideUI();
    }

    public void OnImageLost() {
        if (sentenceState != SentenceState.Animating) return;

        HideArUI();
        HidePreviewUI();
        ShowScanGuideUI();
    }

    public void Play() {
        CurrentModelTarget.TogglePlayPause(false);
    }

    public void Pause() {
        CurrentModelTarget.TogglePlayPause(true);
    }

    public void ResetCurrent() {
        ResetAnimationsInOrder();
        StartCoroutine(PlayAnimationsInOrder());
    }

    public void OnResetRotation() {
        CurrentModelTarget.ResetRotation();
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
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
}
