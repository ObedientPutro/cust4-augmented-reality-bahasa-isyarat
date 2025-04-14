using System;
using System.Collections;
using UnityEngine;

public class ModelTarget : MonoBehaviour {
    [SerializeField] private Animator animator;
    [SerializeField] private float rotationSpeed = 50f;
    
    private Coroutine freezeCoroutine;
    private bool isPaused = false;
    private float animationSpeed = 0.5f;
    private float savedSpeed = 1.0f;
    
    public void PlayCurrentAnimation() {
        ResetAnimatorState();
        animator.speed = animationSpeed;
        isPaused = false;
        freezeCoroutine = StartCoroutine(FreezeAtAnimationEnd());
    }

    public void StopAndResetAnimation() {
        if (freezeCoroutine != null) {
            StopCoroutine(freezeCoroutine);
            freezeCoroutine = null;
        }

        animator.speed = 0;
        isPaused = true;

        ResetAnimatorState();
    }

    public void ResetAnimation() {
        StopAndResetAnimation();
        PlayCurrentAnimation();
    }

    private void ResetAnimatorState() {
        animator.Rebind();
        animator.Update(0f);
    }

    private IEnumerator FreezeAtAnimationEnd() {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        while (state.normalizedTime < 0.98f) {
            state = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        animator.speed = 0;
        isPaused = true;
    }

    public void TogglePlayPause() {
    if (isPaused) {
        // Resume animation
        animator.speed = animationSpeed;
        isPaused = false;

        if (freezeCoroutine != null) {
            StopCoroutine(freezeCoroutine);
        }
        freezeCoroutine = StartCoroutine(FreezeAtAnimationEnd());
    } else {
        // Pause animation
        animator.speed = 0;
        isPaused = true;

        if (freezeCoroutine != null) {
            StopCoroutine(freezeCoroutine);
            freezeCoroutine = null;
        }
    }
}
    
    public void RotateLeft() {
        transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0, Space.World);
    }
    
    public void RotateRight() {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
    }
    
    public void RotateUp() {
        transform.Rotate(-rotationSpeed * Time.deltaTime, 0, 0, Space.World);
    }
    
    public void RotateDown() {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0, Space.World);
    }
    
    // For continuous rotation (hold button)
    public void StartRotateLeft() {
        StartCoroutine(ContinuousRotate(Vector3.down));
    }
    
    public void StartRotateRight() {
        StartCoroutine(ContinuousRotate(Vector3.up));
    }
    
    public void StartRotateUp() {
        StartCoroutine(ContinuousRotate(Vector3.left));
    }
    
    public void StartRotateDown() {
        StartCoroutine(ContinuousRotate(Vector3.right));
    }
    
    public void StopRotation() {
        StopAllCoroutines();
        if (!isPaused && animator.speed > 0) {
            freezeCoroutine = StartCoroutine(FreezeAtAnimationEnd());
        }
    }
    
    private IEnumerator ContinuousRotate(Vector3 direction) {
        while (true) {
            transform.Rotate(direction * rotationSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
    }
}