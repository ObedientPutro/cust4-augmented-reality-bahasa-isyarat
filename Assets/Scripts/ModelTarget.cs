using System;
using System.Collections;
using UnityEngine;

public class ModelTarget : MonoBehaviour {

    [Header("Animation References")]
    [SerializeField] private Animator animator;
    [SerializeField] private float animationSpeed = 0.5f;

    [Space(10)]
    [Header("Rotation References")]
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Vector3 defaultRotationEuler;
    [SerializeField] private float rotationSpeed = 50f;
    
    private Coroutine freezeCoroutine;
    private bool isPaused = false;
    private Coroutine rotationCoroutine;
    
    public void PlayCurrentAnimation() {
        ResetAnimatorState();
        animator.speed = animationSpeed;
        isPaused = false;
        freezeCoroutine = StartCoroutine(FreezeAtAnimationEnd());
    }

    public void SetAnimatorController(RuntimeAnimatorController controller) {
        animator.runtimeAnimatorController = controller;
        ResetAnimatorState();
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

    public void TogglePlayPause(bool pause) {
        if (isPaused && !pause) {
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
    
    public void StartContinuousRotate(Vector3 axis) {
        StopRotation(); // Stop existing coroutine if any
        rotationCoroutine = StartCoroutine(RotateRoutine(axis));
    }

    public void StopRotation() {
        if (rotationCoroutine != null) {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
    }

    private IEnumerator RotateRoutine(Vector3 axis) {
        while (true) {
            Rotate(axis);
            yield return null;
        }
    }

    public void Rotate(Vector3 axis) {
        float deltaRotation = rotationSpeed * Time.deltaTime;

        // Only allow rotation around X and Y axis (no Z axis rotation)
        if (axis == Vector3.up || axis == Vector3.down) {
            transform.Rotate(0f, axis == Vector3.up ? deltaRotation : -deltaRotation, 0f, Space.Self);
        } else if (axis == Vector3.left || axis == Vector3.right) {
            transform.Rotate(axis == Vector3.left ? deltaRotation : -deltaRotation, 0f, 0f, Space.Self);
        }
    }

    public void ResetRotation() {
        transform.localRotation = Quaternion.Euler(defaultRotationEuler);
    }

    public float GetAnimationSpeed() {
        return animationSpeed;
    }
}