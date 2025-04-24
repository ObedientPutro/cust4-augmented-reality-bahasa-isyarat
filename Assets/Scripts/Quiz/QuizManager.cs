using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour {
    public static QuizManager Instance { get; private set; }

    public enum QuizManagerState {
        Idle,
        Playing,
        QuestionTransition,
        GameOver,
        GameFinished,
        GamePaused,
    }

    public event EventHandler<OnQuestionUpdatedEventArgs> OnQuestionUpdated;
    public class OnQuestionUpdatedEventArgs : EventArgs {
        public QuizQuestionSO quizQuestionSO;
    }

    public event EventHandler<OnAttemptsUpdatedEventArgs> OnAttemptsUpdated;
    public class OnAttemptsUpdatedEventArgs : EventArgs {
        public int attempsLeft;
    }

    public event EventHandler<OnTimerUpdatedEventArgs> OnTimerUpdated;
    public class OnTimerUpdatedEventArgs : EventArgs {
        public float timerLeft;
    }

    public event EventHandler<OnAnswerCheckedEventArgs> OnAnswerChecked;
    public class OnAnswerCheckedEventArgs : EventArgs {
        public int buttonIndex;
        public Boolean isCorrect;
    }

    public event EventHandler<OnGameFinsihedEventArgs> OnGameFinsihed;
    public class OnGameFinsihedEventArgs : EventArgs {
        public int currentScore;
        public int totalScore;
    }

    [Header("References")]
    [SerializeField] private int totalQuizPerLevel = 10;
    [SerializeField] private int scoreMultiplier = 10;
    [SerializeField] private int attemptLimit = 3;
    [SerializeField] private float timeLimitPerQuestion = 10f;
    [SerializeField] private float transitionTime = 2f;

    [Space(10)]
    [Header("UI References")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject quizUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject gameFinishUI;
    [SerializeField] private GameObject gameOverUI;

    [Space(10)]
    [Header("Questions")]
    [SerializeField] private List<QuizQuestionSO> questions = new List<QuizQuestionSO>();

    private List<QuizQuestionSO> currentLevel = new List<QuizQuestionSO>();
    private QuizQuestionSO currentQuestion;
    private int currentQuestionIndex = 0;
    private float timer;
    private QuizManagerState state = QuizManagerState.Idle;
    private int correctAnswers = 0;
    private int attemptsLeft = 0;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        state = QuizManagerState.Idle;
    }

    private void Update() {
        if (state == QuizManagerState.Playing) {
            timer -= Time.deltaTime;
            UpdateTimerNormalized();

            if (timer <= 0) {
                SetQuizStateToGameOver(); 
            }
        }
    }

    private void LoadQuestion() {
        if (currentQuestionIndex < totalQuizPerLevel) {
            state = QuizManagerState.Playing;
            timer = timeLimitPerQuestion;
            UpdateTimerNormalized();
            UpdateQuizUI();
        } else {
            SetQuizStateToGameFinish();
        }
    }

    private IEnumerator CurrentQuestionTransition() {
        yield return new WaitForSeconds(transitionTime);
        state = QuizManagerState.Playing;
        LoadQuestion();
    }

    private void UpdateTimerNormalized() {
        float normalizedTimer = timer / timeLimitPerQuestion;
        OnTimerUpdated?.Invoke(this, new OnTimerUpdatedEventArgs { 
            timerLeft = normalizedTimer 
        });
    }

    private void SetQuizStateToGameOver() {
        state = QuizManagerState.GameOver;
        // SoundManager.Instance.StopBGM();
        // SoundManager.Instance.PlaySFX("game_over_sound");
        ShowGameOver();
        state = QuizManagerState.Idle;
    }

    private void SetQuizStateToGameFinish() {
        state = QuizManagerState.GameFinished;
        OnGameFinsihed?.Invoke(this, new OnGameFinsihedEventArgs {
            currentScore = correctAnswers * scoreMultiplier,
            totalScore = totalQuizPerLevel * scoreMultiplier,
        });
        // SoundManager.Instance.StopBGM();
        // SoundManager.Instance.PlaySFX("complete_sound");
        ShowGameFinish();
        state = QuizManagerState.Idle;
    }

    private void UpdateQuizUI() {
        OnQuestionUpdated?.Invoke(this, new OnQuestionUpdatedEventArgs {
            quizQuestionSO = currentLevel[currentQuestionIndex],
        });
        OnAttemptsUpdated?.Invoke(this, new OnAttemptsUpdatedEventArgs {
            attempsLeft = attemptsLeft
        });
    }

    public void StartLevel() {
        currentLevel = questions.OrderBy(x => UnityEngine.Random.value).Take(totalQuizPerLevel).ToList();
        correctAnswers = 0;
        currentQuestionIndex = 0;
        attemptsLeft = attemptLimit;
        PlayQuizMusicBGM();
        ShowQuiz();
        LoadQuestion();
    }

    public void CheckAnswer(int selectedIndex) {
        QuizQuestionSO currentQuestion = currentLevel[currentQuestionIndex];

        OnAnswerChecked?.Invoke(this, new OnAnswerCheckedEventArgs {
            buttonIndex = selectedIndex,
            isCorrect = currentQuestion.answers[selectedIndex].isCorrect,
        });

        if (selectedIndex >= 0 && currentQuestion.answers[selectedIndex].isCorrect) {
            // Correct Answer
            // SoundManager.Instance.PlaySFX("correct_sound");
            correctAnswers++;
        } else {
            // Wrong Answer
            // SoundManager.Instance.PlaySFX("wrong_sound");
            attemptsLeft--;
            OnAttemptsUpdated?.Invoke(this, new OnAttemptsUpdatedEventArgs {
                attempsLeft = attemptsLeft
            });
        }

        if (attemptsLeft <= 0) {
            SetQuizStateToGameOver();
            return;
        }

        currentQuestionIndex++;
        state = QuizManagerState.QuestionTransition;
        StartCoroutine(CurrentQuestionTransition());
    }

    public void RestartGame() {
        StartLevel();
    }

    public void ShowQuiz() {
        menuUI.SetActive(false);
        quizUI.SetActive(true);
        settingsUI.SetActive(false);
        gameFinishUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    public void ShowMenu() {
        PlayMusicBGM();
        state = QuizManagerState.Idle;
        menuUI.SetActive(true);
        quizUI.SetActive(false);
        settingsUI.SetActive(false);
        gameFinishUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    public void ShowSetting() {
        menuUI.SetActive(false);
        quizUI.SetActive(false);
        settingsUI.SetActive(true);
        gameFinishUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    public void ShowGameOver() {
        menuUI.SetActive(false);
        quizUI.SetActive(true);
        settingsUI.SetActive(false);
        gameFinishUI.SetActive(false);
        gameOverUI.SetActive(true);
    }

    public void ShowGameFinish() {
        menuUI.SetActive(false);
        quizUI.SetActive(true);
        settingsUI.SetActive(false);
        gameFinishUI.SetActive(true);
        gameOverUI.SetActive(false);
    }

    public void PlayMusicBGM() {
        // SoundManager.Instance.PlayBGM("bgm_music");
    }

    public void PlayQuizMusicBGM() {
        // SoundManager.Instance.PlayBGM("quiz_bgm_music");
    }

    public void PlayClickSFX() {
        // SoundManager.Instance.PlaySFX("click_sound");
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
