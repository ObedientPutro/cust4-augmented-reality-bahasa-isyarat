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
        Screen.orientation = ScreenOrientation.Portrait;
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
        SoundManager.Instance.PlayButtonSoundEffect();
        SetAllMenusInactive();
        mainMenu.SetActive(true);
    }

    public void ShowScanMenu() {
        SoundManager.Instance.PlayButtonSoundEffect();
        SetAllMenusInactive();
        scanMenu.SetActive(true);
    }

    public void ShowSettingsMenu() {
        SoundManager.Instance.PlayButtonSoundEffect();
        settingsMenu.SetActive(true);
    }

    public void ShowAboutMenu() {
        SoundManager.Instance.PlayButtonSoundEffect();
        SetAllMenusInactive();
        aboutMenu.SetActive(true);
    }

    public void ShowTutorialMenu() {
        SoundManager.Instance.PlayButtonSoundEffect();
        SetAllMenusInactive();
        tutorialMenu.SetActive(true);
    }

    public void ExitApplication() {
        SoundManager.Instance.PlayButtonSoundEffect();
        Debug.Log("Application Exited");
        Application.Quit();
    }

    public void GoToAlphabetScene() {
        SoundManager.Instance.PlayButtonSoundEffect();
        SceneManager.LoadScene("Alphabet");
    }

    public void GoToWordScene() {
        SoundManager.Instance.PlayButtonSoundEffect();
        SceneManager.LoadScene("Word");
    }

    public void GoToSentenceScene() {
        SoundManager.Instance.PlayButtonSoundEffect();
        SceneManager.LoadScene("Sentence");
    }

    public void GoToQuizScene() {
        SoundManager.Instance.PlayButtonSoundEffect();
        SceneManager.LoadScene("Quiz");
    }

}
