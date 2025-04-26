using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizScanManager : MonoBehaviour {
    public static QuizScanManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject ARCamera;

    private CardIdentity scannedCard = null;
    private bool isScanning = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        DisableScanMode();
    }

    public void ActivateScanMode() {
        ARCamera.SetActive(true);
        isScanning = true;
    }

    public void DisableScanMode() {
        ARCamera.SetActive(false);
        isScanning = false;
    }

    public void OnCardFound(CardIdentity card) {
        if (isScanning == false) return;
        if (scannedCard != null) return;
        scannedCard = card;
        QuizManager.Instance.CheckScanAnswer(card);
    }

    public void OnCardLost() {
        scannedCard = null;
    }

}
