using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphabetManager : MonoBehaviour {
    [SerializeField] private ModelTarget modelTarget;
    [SerializeField] private Image signImage;
    [SerializeField] private Animator animator;
    [SerializeField] private List<AlphabetData> signDataList;

    private int currentIndex = 0;

    private void Start() {
        LoadCurrentSign();
    }

    public void NextSign() {
        currentIndex = (currentIndex + 1) % signDataList.Count;
        LoadCurrentSign();
    }

    public void PreviousSign() {
        currentIndex = (currentIndex - 1 + signDataList.Count) % signDataList.Count;
        LoadCurrentSign();
    }

    public void TogglePlayPause() {
        modelTarget.TogglePlayPause();
    }

    public void PlayCurrent() {
        modelTarget.PlayCurrentAnimation();
    }

    public void StopCurrent() {
        modelTarget.StopAndResetAnimation();
    }

    public void ResetCurrent() {
        modelTarget.ResetAnimation();
    }

    private void LoadCurrentSign() {
        AlphabetData data = signDataList[currentIndex];

        // Set sprite
        signImage.sprite = data.image;

        // Play animation directly
        animator.runtimeAnimatorController = CreateControllerForClip(data.clip);
        modelTarget.PlayCurrentAnimation();
    }

    // Dynamically create a simple controller for one clip
    private RuntimeAnimatorController CreateControllerForClip(AnimationClip clip) {
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        overrideController["__default__"] = clip;
        return overrideController;
    }
}
