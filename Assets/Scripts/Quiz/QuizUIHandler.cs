using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizUIHandler : MonoBehaviour {
    public static QuizUIHandler Instance { get; private set; }

    public event EventHandler<OnChangeButtonAnswerCheckedEventArgs> OnChangeButtonAnswerChecked;
    public class OnChangeButtonAnswerCheckedEventArgs : EventArgs {
        public int buttonIndex;
        public Sprite spriteIndicator;
        public Boolean isCorrect;
    }

    [Header("Core UI")]
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private Slider timerSlider;

    [Space(5)]
    [Header("Question Text")]
    [SerializeField] private TextMeshProUGUI questionText;


    [Space(5)]
    [Header("Answer")]
    [SerializeField] private GameObject textAnswerContainer;
    [SerializeField] private GameObject scanAnswerContainer;
    [SerializeField] private List<QuizButtonAnswer> answerButtons;

    [Space(5)]
    [Header("Answer Indicator")]
    [SerializeField] private Sprite correctIndicator;
    [SerializeField] private Sprite wrongIndicator;
    [SerializeField] private Image scanIndicatorImage;

    [Space(5)]
    [Header("Game Finish UI")]
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI totalScore;

    [Space(5)]
    [Header("UI References")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject quizContainer;
    [SerializeField] private GameObject quizUI;
    [SerializeField] private GameObject scanUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject gameFinishUI;
    [SerializeField] private GameObject gameOverUI;

    private const string BUTTON_BUBBLE = "button_bubble";

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }
    
    private void Start() {
        ShowMenu();

        QuizManager.Instance.OnQuestionUpdated += QuizManager_OnQuestionUpdated;
        QuizManager.Instance.OnAttemptsUpdated += QuizManager_OnAttemptsUpdated;
        QuizManager.Instance.OnTimerUpdated += QuizManager_OnTimerUpdated;
        QuizManager.Instance.OnAnswerChecked += QuizManager_OnAnswerChecked;
        QuizManager.Instance.OnGameFinsihed += QuizManager_OnGameFinsihed;
        QuizManager.Instance.OnStateChanged += QuizManager_OnStateChanged;
        QuizManager.Instance.OnCorrectScan += QuizManager_OnCorrectScan;
        QuizManager.Instance.OnWrongScan += QuizManager_OnWrongScan;
    }

    private void OnDestroy() {
        QuizManager.Instance.OnQuestionUpdated -= QuizManager_OnQuestionUpdated;
        QuizManager.Instance.OnAttemptsUpdated -= QuizManager_OnAttemptsUpdated;
        QuizManager.Instance.OnTimerUpdated -= QuizManager_OnTimerUpdated;
        QuizManager.Instance.OnAnswerChecked -= QuizManager_OnAnswerChecked;
        QuizManager.Instance.OnGameFinsihed -= QuizManager_OnGameFinsihed;
        QuizManager.Instance.OnStateChanged -= QuizManager_OnStateChanged;
        QuizManager.Instance.OnCorrectScan -= QuizManager_OnCorrectScan;
        QuizManager.Instance.OnWrongScan -= QuizManager_OnWrongScan;
    }

    private void QuizManager_OnQuestionUpdated(object sender, QuizManager.OnQuestionUpdatedEventArgs e) {
        ShowQuiz();

        QuizQuestionSO question = e.quizQuestionSO;
        questionText.text = question.question;

        if (question.type == QuizType.MultipleChoice) {
            scanAnswerContainer.SetActive(false);
            textAnswerContainer.SetActive(true);
            
            List<QuizAnswer> shuffledAnswers = question.answers.OrderBy(a => UnityEngine.Random.value).ToList();

            int index = 0;
            foreach (QuizButtonAnswer quizButtonAnswer in answerButtons) {
                quizButtonAnswer.Setup(
                    shuffledAnswers[index].text, 
                    question.answers.IndexOf(shuffledAnswers[index]), 
                    QuizManager.Instance.CheckButtonAnswers
                );
                index++;
            }
        } else {
            scanAnswerContainer.SetActive(true);
            textAnswerContainer.SetActive(false);
        }
    }

    private void QuizManager_OnAttemptsUpdated(object sender, QuizManager.OnAttemptsUpdatedEventArgs e) {
        attemptsText.text = e.attempsLeft.ToString();
    }

    private void QuizManager_OnTimerUpdated(object sender, QuizManager.OnTimerUpdatedEventArgs e) {
        timerSlider.value = e.timerLeft;
    }

    private void QuizManager_OnAnswerChecked(object sender, QuizManager.OnAnswerCheckedEventArgs e) {
        OnChangeButtonAnswerChecked?.Invoke(this, new OnChangeButtonAnswerCheckedEventArgs{
            buttonIndex = e.buttonIndex,
            spriteIndicator = e.isCorrect ? correctIndicator : wrongIndicator,
            isCorrect = e.isCorrect,
        });
    }

    private void QuizManager_OnGameFinsihed(object sender, QuizManager.OnGameFinsihedEventArgs e) {
        ShowGameFinish();
        score.text = e.currentScore.ToString();
        totalScore.text = e.totalScore.ToString();
    }

    private void QuizManager_OnStateChanged(object sender, QuizManager.QuizManagerState e) {
        switch (e) {
            case QuizManager.QuizManagerState.GameOver:
                ShowGameOver();
                break;
            default:
                return;
        }
    }

    private void QuizManager_OnWrongScan(object sender, EventArgs e) {
        StartCoroutine(ShowScanIndicator(wrongIndicator));
    }

    private void QuizManager_OnCorrectScan(object sender, EventArgs e) {
        StartCoroutine(ShowScanIndicator(correctIndicator));
    }

    private IEnumerator ShowScanIndicator(Sprite indicatorSprite) {
        scanIndicatorImage.transform.gameObject.SetActive(true);
        scanIndicatorImage.sprite = indicatorSprite;
        scanIndicatorImage.preserveAspect = true;
        yield return new WaitForSeconds(1);
        scanIndicatorImage.transform.gameObject.SetActive(false);
    }

    private void SetInactiveAll() {
        menuUI.SetActive(false);
        quizContainer.SetActive(false);
        settingsUI.SetActive(false);
        gameFinishUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    private void ShowGameOver() {
        SetInactiveAll();
        gameOverUI.SetActive(true);
    }

    private void ShowGameFinish() {
        SetInactiveAll();
        gameFinishUI.SetActive(true);
    }

    private void ShowMenu() {
        SetInactiveAll();
        menuUI.SetActive(true);
    }

    private void ShowQuiz() {
        SetInactiveAll();
        quizContainer.SetActive(true);
        SwitchToQuizMode();
    }

    private void ShowSettings() {
        settingsUI.SetActive(true);
    }

    private void HideSettings() {
        settingsUI.SetActive(false);
    }

    public void SwitchToQuizMode() {
        quizUI.SetActive(true);
        scanUI.SetActive(false);
        QuizScanManager.Instance.DisableScanMode();
    }

    public void SwitchToScanMode() {
        quizUI.SetActive(false);
        scanUI.SetActive(true);
        QuizScanManager.Instance.ActivateScanMode();
    }

    public void BackToMainMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        QuizManager.Instance.SetQuizToIdle();
        QuizScanManager.Instance.DisableScanMode();
        ShowMenu();
    }

    public void SettingsButton() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        ShowSettings();
    }

    public void CloseSettingsButton() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        HideSettings();
    }

    public void StartButton() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        ShowQuiz();
        QuizManager.Instance.StartLevel();
    }

    public void PlayAnimationButton() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        QuizManager.Instance.Play();
    }

    public void PauseAnimationButton() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        QuizManager.Instance.Pause();
    }

    public void ResetAnimationButton() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        QuizManager.Instance.ResetCurrent();
    }

}
