using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quiz/Quiz Question")]
public class QuizQuestionSO : ScriptableObject {
    public QuizType type = QuizType.MultipleChoice;
    public string question = "Enter new question text here";

    [Space(10)]
    [Header("Animation should be in order")]
    public List<RuntimeAnimatorController> animationControllers = new List<RuntimeAnimatorController>();

    [Space(10)]
    [Header("Answers should be 4 (Multiple Choice) or 1 (Scan Mode)")]
    public List<QuizAnswer> answers = new List<QuizAnswer>();
}
