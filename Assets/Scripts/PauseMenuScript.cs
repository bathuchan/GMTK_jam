using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public bool GamePaused = false;
    bool isSettingsOpen = false;

    public GameObject pauseMenuUI, settingsMenuUI, headsUpDisplay;

    private void Awake()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        headsUpDisplay.SetActive(true);
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);headsUpDisplay.SetActive(true);
        Time.timeScale = 1f;
        GamePaused = false;
    }
    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);headsUpDisplay.SetActive(false);
        Time.timeScale = 0f;
        GamePaused = true;
    }
    public void Settings()
    {
        if (!isSettingsOpen) 
        {
            isSettingsOpen = true;
            settingsMenuUI.SetActive(true);
        }
        else
        {
            isSettingsOpen = false;
            settingsMenuUI.SetActive(false);
        }
    }
    public void Restart()
    {
        Debug.Log("Restarting level...");
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);        
    }
}
