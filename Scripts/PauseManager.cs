using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public GameObject wisdomScrollPanel;

    [Header("Buttons")]
    public Button pauseButton;
    public Button resumeButton;
    public Button wisdomScrollButton;
    public Button returnButton;
    public Button backFromScrollButton;

    private bool isPaused = false;

    void Awake()
    {
        // Disable both UI panels on start
        pauseMenuPanel.SetActive(false);
        wisdomScrollPanel.SetActive(false);

        // Make sure time runs normally
        Time.timeScale = 1f;
    }

    void Start()
    {
        // Assign button click listeners
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        wisdomScrollButton.onClick.AddListener(OpenWisdomScroll);
        returnButton.onClick.AddListener(ReturnToMainMenu);
        backFromScrollButton.onClick.AddListener(BackToPauseMenu);
    }

public void PauseGame()
{
    if (!isPaused)
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);  // Show UI first
        StartCoroutine(DelayPause());    // Then pause time
    }
}

private IEnumerator DelayPause()
{
    yield return new WaitForEndOfFrame(); // Let the frame render with UI enabled
    Time.timeScale = 0f;
}


    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
    }

    public void OpenWisdomScroll()
    {
        pauseMenuPanel.SetActive(false);
        wisdomScrollPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        wisdomScrollPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Change if your main menu has a different name
    }
}
