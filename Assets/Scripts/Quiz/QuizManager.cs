using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour {
    public static QuizManager Instance { get; private set; }

    public enum QuizManagerState {
        Idle,
        Playing,
        QuestionTransition,
        GameOver,
        GameFinished,
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

    public event EventHandler OnWrongScan;
    public event EventHandler OnCorrectScan;

    public event EventHandler<QuizManagerState> OnStateChanged;

    [Header("References")]
    [SerializeField] private int totalQuizPerLevel = 10;
    [SerializeField] private int scoreMultiplier = 10;
    [SerializeField] private int attemptLimit = 3;
    [SerializeField] private float timeLimitPerQuestion = 10f;
    [SerializeField] private float transitionTime = 2f;
    [SerializeField] private ModelTarget modelTarget;

    [Space(10)]
    [Header("Questions")]
    [SerializeField] private List<QuizQuestionSO> questions = new List<QuizQuestionSO>();

    private List<QuizQuestionSO> currentLevel = new List<QuizQuestionSO>();
    private int currentQuestionIndex = 0;
    private float timer;
    private QuizManagerState state;
    private int correctAnswers = 0;
    private int attemptsLeft = 0;

    private const string MAIN_BGM = "main_backsound";
    private const string QUIZ_BGM = "quiz_backsound";

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        Screen.orientation = ScreenOrientation.Portrait;
        SetQuizManagerState(QuizManagerState.Idle);
    }

    private void Update() {
        if (state == QuizManagerState.Playing) {
            timer -= Time.deltaTime;
            UpdateTimerNormalized();

            if (timer <= 0) {
                SetQuizManagerState(QuizManagerState.GameOver); 
            }
        }
    }

    private void SetQuizManagerState(QuizManagerState state) {
        this.state = state;
        switch (state) {
            case QuizManagerState.Idle:
                SoundManager.Instance.PlayBGM(MAIN_BGM);
                break;
            case QuizManagerState.Playing:
                SoundManager.Instance.PlayBGM(QUIZ_BGM);
                break;
            case QuizManagerState.QuestionTransition:
                StartCoroutine(CurrentQuestionTransition());
                break;
            case QuizManagerState.GameOver:
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlaySFX("game_over_sound");
                break;
            case QuizManagerState.GameFinished:
                OnGameFinsihed?.Invoke(this, new OnGameFinsihedEventArgs {
                    currentScore = correctAnswers * scoreMultiplier,
                    totalScore = totalQuizPerLevel * scoreMultiplier,
                });
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlaySFX("complete_sound");
                break;
        }
        OnStateChanged?.Invoke(this, state);
    }

    private void NewListRandomQuestions() {
        currentLevel = questions.OrderBy(x => UnityEngine.Random.value).Take(totalQuizPerLevel).ToList();
    }

    private IEnumerator CurrentQuestionTransition() {
        yield return new WaitForSeconds(transitionTime);
        SetQuizManagerState(QuizManagerState.Playing);
        LoadQuestion();
    }

    private void UpdateTimerNormalized() {
        float normalizedTimer = timer / timeLimitPerQuestion;
        OnTimerUpdated?.Invoke(this, new OnTimerUpdatedEventArgs { 
            timerLeft = normalizedTimer 
        });
    }

    private void UpdateAttempsLeft(int attempsLeft) {
        this.attemptsLeft = attempsLeft;
        OnAttemptsUpdated?.Invoke(this, new OnAttemptsUpdatedEventArgs {
            attempsLeft = attempsLeft,
        });
    }

    private void LoadQuestion() {
        if (currentQuestionIndex < totalQuizPerLevel) {
            timer = timeLimitPerQuestion;
            OnQuestionUpdated?.Invoke(this, new OnQuestionUpdatedEventArgs {
                quizQuestionSO = currentLevel[currentQuestionIndex],
            });
            PlayAnimation();
        } else {
            SetQuizManagerState(QuizManagerState.GameFinished);
        }
    }

    private void CorrectAnswer() {
        SoundManager.Instance.PlaySFX("correct_sound");
        correctAnswers++;
        if (currentLevel[currentQuestionIndex].type == QuizType.ScanMode) OnCorrectScan?.Invoke(this, EventArgs.Empty);
    }

    private void WrongAnswer() {
        SoundManager.Instance.PlaySFX("wrong_sound");
        UpdateAttempsLeft(attemptsLeft - 1);
        if (currentLevel[currentQuestionIndex].type == QuizType.ScanMode) OnWrongScan?.Invoke(this, EventArgs.Empty);
    }

    private bool HasAttemptsLeft() {
        if (attemptsLeft <= 0) {
            SetQuizManagerState(QuizManagerState.GameOver);
            return false;
        }
        return true;
    }

    private void MoveToNextQuestion() {
        currentQuestionIndex++;
        SetQuizManagerState(QuizManagerState.QuestionTransition);
    }

    private void PlayAnimation() {
        QuizQuestionSO currentQuestion = currentLevel[currentQuestionIndex];
        StartCoroutine(PlayAnimationsInOrder(currentQuestion));
    }

    private IEnumerator<WaitForSeconds> PlayAnimationsInOrder(QuizQuestionSO currentQuestion) {
        foreach (RuntimeAnimatorController controller in currentQuestion.animationControllers) {
            AnimationClip clip = controller.animationClips.Length > 0 ? controller.animationClips[0] : null;
            if (clip != null) {
                modelTarget.SetAnimatorController(controller);
                modelTarget.PlayCurrentAnimation();
                
                float duration = clip.length / modelTarget.GetAnimationSpeed();
                yield return new WaitForSeconds(duration);
            }
        }
    }

    public void StartLevel() {
        NewListRandomQuestions();
        correctAnswers = 0;
        currentQuestionIndex = 0;
        UpdateAttempsLeft(attemptLimit);
        SetQuizManagerState(QuizManagerState.Playing);
        LoadQuestion();
    }

    public void CheckButtonAnswers(int selectedIndex) {
        if (state != QuizManagerState.Playing) return;
        
        QuizQuestionSO currentQuestion = currentLevel[currentQuestionIndex];
        OnAnswerChecked?.Invoke(this, new OnAnswerCheckedEventArgs {
            buttonIndex = selectedIndex,
            isCorrect = currentQuestion.answers[selectedIndex].isCorrect,
        });

        if (selectedIndex >= 0 && currentQuestion.answers[selectedIndex].isCorrect) CorrectAnswer();
        else WrongAnswer();

        if (!HasAttemptsLeft()) return;
        MoveToNextQuestion();
    }

    public void CheckScanAnswer(CardIdentity card) {
        if (state != QuizManagerState.Playing) return;

        QuizQuestionSO currentQuestion = currentLevel[currentQuestionIndex];
        string correctAnswer = currentQuestion.answers.FirstOrDefault(a => a.isCorrect).text;

        if (card.word.ToLower() == correctAnswer.ToLower()) CorrectAnswer();
        else WrongAnswer();

        if (!HasAttemptsLeft()) return;
        MoveToNextQuestion();
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

    public void SetQuizToIdle() {
        SetQuizManagerState(QuizManagerState.Idle);
    }

}
