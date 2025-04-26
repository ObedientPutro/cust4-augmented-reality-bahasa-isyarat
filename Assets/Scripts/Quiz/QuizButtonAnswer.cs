using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizButtonAnswer : MonoBehaviour {
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private Image indicatorImage;
    private int answerIndex;

    private void Start() {
        QuizUIHandler.Instance.OnChangeButtonAnswerChecked += QuizUIHandler_OnChangeButtonAnswerChecked;
    }

    private void OnDisable() {
        answerIndex = -1;
    }

    private void QuizUIHandler_OnChangeButtonAnswerChecked(object sender, QuizUIHandler.OnChangeButtonAnswerCheckedEventArgs e) {
        button.interactable = false;
        if (e.buttonIndex != answerIndex) return;
        StartCoroutine(ShowAnswerIndicator(e));
    }

    private IEnumerator ShowAnswerIndicator(QuizUIHandler.OnChangeButtonAnswerCheckedEventArgs e) {
        indicatorImage.transform.gameObject.SetActive(true);
        indicatorImage.sprite = e.spriteIndicator;
        indicatorImage.preserveAspect = true;
        yield return new WaitForSeconds(1);
        indicatorImage.transform.gameObject.SetActive(false);
    }

    public void Setup(string text, int index, System.Action<int> callback) {
        button.interactable = true;
        answerText.text =  (answerText != null) ? text : "";
        indicatorImage.transform.gameObject.SetActive(false);
        
        answerIndex = index;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback.Invoke(answerIndex));
    }
    
}
