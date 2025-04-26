using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject scanMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject aboutMenu;
    [SerializeField] private GameObject tutorialMenu;

    private const string BUTTON_BUBBLE = "button_bubble";

    private void Start() {
        SoundManager.Instance.PlayBGM("main_backsound");
        Screen.orientation = ScreenOrientation.Portrait;
        
        SetAllMenusInactive();
        mainMenu.SetActive(true);
    }

    private void SetAllMenusInactive() {
        mainMenu.SetActive(false);
        scanMenu.SetActive(false);
        settingsMenu.SetActive(false);
        aboutMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }

    public void ShowMainMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SetAllMenusInactive();
        mainMenu.SetActive(true);
    }

    public void ShowScanMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SetAllMenusInactive();
        scanMenu.SetActive(true);
    }

    public void ShowSettingsMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        settingsMenu.SetActive(true);
    }

    public void ShowAboutMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SetAllMenusInactive();
        aboutMenu.SetActive(true);
    }

    public void ShowTutorialMenu() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SetAllMenusInactive();
        tutorialMenu.SetActive(true);
    }

    public void ExitApplication() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        Debug.Log("Application Exited");
        Application.Quit();
    }

    public void GoToAlphabetScene() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SceneManager.LoadScene("Alphabet");
    }

    public void GoToWordScene() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SceneManager.LoadScene("Word");
    }

    public void GoToSentenceScene() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SceneManager.LoadScene("Sentence");
    }

    public void GoToQuizScene() {
        SoundManager.Instance.PlaySFX(BUTTON_BUBBLE);
        SceneManager.LoadScene("Quiz");
    }

}
