using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] string nextScene;

    [SerializeField] GameObject dialoguePanelButton;

    [SerializeField] bool isInMainMenu;

    [SerializeField] GameObject deadMenu;
    [SerializeField] GameObject menuPanel;

    [SerializeField] EventSystem eventSystem;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject endMenu;

    [SerializeField] GameObject fadePanel;
    [SerializeField] GameObject levelSelectMenu;
    [SerializeField] GameObject settingsMenu;

    [Header("Audio")]
    [SerializeField] AudioSource runAudioSource;
    [SerializeField] AudioClip soundsToPlay;
    [SerializeField, Range(0, 1)] float soundVolume = 1;

    bool isMenuSelected;
    [HideInInspector] public bool isInPause;
    bool isRetrying;
    bool isGoingToMainMenu;
    bool isPlaying;
    bool isQuitting;
    bool nextLevel;
    bool wasPlaying;

    bool isMenuButtonSelected;

    private void Start()
    {
        isMenuSelected = false;
        Time.timeScale = 1f;

        fadePanel.SetActive(true);
    }

    private void Update()
    {
        if (isInMainMenu && !isMenuButtonSelected)
        {
            isMenuButtonSelected = true;
            eventSystem.SetSelectedGameObject(pauseMenu.transform.GetChild(0).gameObject);
        }

        if (isInMainMenu) return;

        if (FindObjectOfType<DialogueManager>().isTalking)
        {
            eventSystem.SetSelectedGameObject(dialoguePanelButton);
            fadePanel.SetActive(false);
            return;
        }
        else if(eventSystem.currentSelectedGameObject == dialoguePanelButton)
        {
            eventSystem.SetSelectedGameObject(null);
        }

        if (Input.GetKeyDown(KeyCode.R) && !isInMainMenu) Retry();

        if (deadMenu != null)
        {
            if (deadMenu.activeSelf == true && !isMenuSelected)
            {
                fadePanel.SetActive(false);
                AudioManager.instance.gameObject.GetComponent<AudioSource>().Stop();
                AudioManager.instance.PlayClipAt(soundsToPlay, transform, soundVolume);

                if (runAudioSource != null)
                {
                    runAudioSource.GetComponent<AudioSource>().Stop();
                }

                isMenuSelected = true;
                eventSystem.SetSelectedGameObject(deadMenu.transform.GetChild(0).gameObject);
            }
        }

        if (pauseMenu != null && deadMenu.activeSelf == false && endMenu.activeSelf == false)
        {
            if (Input.GetButtonDown("Pause") && !isInPause)
            {
                Pause();
            }
            else if (Input.GetButtonDown("Pause") && isInPause)
            {
                Continue();
            }
        }
    }

    public void Pause()
    {
        isInPause = true;
        fadePanel.SetActive(false);
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(pauseMenu.transform.GetChild(0).gameObject);

        if(runAudioSource != null)
        {
            if(runAudioSource.isPlaying)
            {
                wasPlaying = true;
                runAudioSource.Pause();
            }
            else
            {
                wasPlaying = false;
            }
        }

        Time.timeScale = 0f;

    }

    //public void callWaitForMove()
    //{
    //    StartCoroutine(WaitForMove());
    //}

    //IEnumerator WaitForMove()
    //{
    //    yield return new WaitForSecondsRealtime(0.05f);

    //    Continue();
    //}

    public void SelectMenu()
    {
        eventSystem.SetSelectedGameObject(levelSelectMenu.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject);
        menuPanel.GetComponent<Animator>().SetTrigger("SelectMenu");
    }

    public void NormalMenu()
    {
        eventSystem.SetSelectedGameObject(pauseMenu.transform.GetChild(0).gameObject);
        menuPanel.GetComponent<Animator>().SetTrigger("NormalMenu");
    }

    public void SettingsMenu()
    {
        eventSystem.SetSelectedGameObject(settingsMenu.transform.GetChild(0).gameObject);
        menuPanel.GetComponent<Animator>().SetTrigger("SettingsMenu");
    }

    public void SettingsMenuToNormal()
    {
        eventSystem.SetSelectedGameObject(pauseMenu.transform.GetChild(1).gameObject);
        menuPanel.GetComponent<Animator>().SetTrigger("NormalMenu");
    }

    //IEnumerator DisableSettingsMenu()
    //{
    //    yield return new WaitForSeconds(1);

    //    settingsMenu.SetActive(false);
    //}

    //IEnumerator DisableMenu()
    //{
    //    yield return new WaitForSeconds(1);

    //    pauseMenu.SetActive(false);
    //}

    //IEnumerator DisableSelectMenu()
    //{
    //    yield return new WaitForSeconds(1);

    //    levelSelectMenu.SetActive(false);
    //}

    public void Continue()
    {
        pauseMenu.GetComponent<Animator>().SetTrigger("FadeOut");

        StartCoroutine(EnablePauseMenu());
    }

    IEnumerator EnablePauseMenu()
    {
        yield return new WaitForSecondsRealtime(0.15f);

        isInPause = false;
        pauseMenu.SetActive(false);

        Time.timeScale = 1f;

        if (runAudioSource != null)
        {
            if (wasPlaying)
            {
                runAudioSource.Play();
                wasPlaying = false;
            }
        }
    }

    public void NextLevel()
    {
        nextLevel = true;

        StartCoroutine(FadeIn());
    }

    public void Retry()
    {
        isRetrying = true;

        StartCoroutine(FadeIn());
    }

    public void MainMenu()
    {
        isGoingToMainMenu = true;

        StartCoroutine(FadeIn());
    }

    public void QuitGame()
    {
        isQuitting = true;

        StartCoroutine(FadeIn());
    }

    public void Play()
    {
        isPlaying = true;

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        fadePanel.SetActive(true);

        fadePanel.GetComponent<Animator>().SetTrigger("FadeIn");

        yield return new WaitForSecondsRealtime(1);

        if (isRetrying)
        {
            isRetrying = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (isGoingToMainMenu)
        {
            isGoingToMainMenu = false;
            SceneManager.LoadScene("MainMenu");
        }

        if (isPlaying)
        {
            isPlaying = false;
            SceneManager.LoadScene("LevelSelect");
        }

        if (isQuitting)
        {
            isQuitting = false;
            Application.Quit();
        }

        if (nextLevel)
        {
            nextLevel = false;
            SceneManager.LoadScene(nextScene);
        }
    }
}
