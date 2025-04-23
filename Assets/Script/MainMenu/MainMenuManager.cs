using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject scanMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject aboutMenu;
    [SerializeField] private GameObject tutorialMenu;

    private void Start() {
        ShowMainMenu();
    }

    private void SetAllMenusInactive() {
        mainMenu.SetActive(false);
        scanMenu.SetActive(false);
        settingsMenu.SetActive(false);
        aboutMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }

    public void ShowMainMenu() {
        SetAllMenusInactive();
        mainMenu.SetActive(true);
    }

    public void ShowScanMenu() {
        SetAllMenusInactive();
        scanMenu.SetActive(true);
    }

    public void ShowSettingsMenu() {
        settingsMenu.SetActive(true);
    }

    public void ShowAboutMenu() {
        SetAllMenusInactive();
        aboutMenu.SetActive(true);
    }

    public void ShowTutorialMenu() {
        SetAllMenusInactive();
        tutorialMenu.SetActive(true);
    }

    public void ExitApplication() {
        Debug.Log("Application Exited");
        Application.Quit();
    }

    public void GoToAlphabetScene() {
        SceneManager.LoadScene("Alphabet");
    }

    public void GoToWordScene() {
        SceneManager.LoadScene("Word");
    }

    public void GoToSentenceScene() {
        SceneManager.LoadScene("Sentence");
    }

    public void GoToQuizScene() {
        // SceneManager.LoadScene("Quiz");
    }

}
