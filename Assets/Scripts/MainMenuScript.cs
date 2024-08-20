using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public bool GamePaused = false;
    bool isSettingsOpen = false, isCreditsOpen = false, isLevelsOpen = false;

    public GameObject mainMenuUI, settingsMenuUI, creditsMenuUI, levelsMenuUI;

    private void Awake()
    {
        mainMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(false);
    }
    public void Levels()
    {
        if (!isLevelsOpen)
        {
            isLevelsOpen = true;
            levelsMenuUI.SetActive(true);
            mainMenuUI.SetActive(false);
        }
        else
        {
            isLevelsOpen = false;
            levelsMenuUI.SetActive(false);
            mainMenuUI.SetActive(true);
        }
    }
    public void Settings()
    {
        if (!isSettingsOpen)
        {
            isSettingsOpen = true;
            settingsMenuUI.SetActive(true);
            mainMenuUI.SetActive(false);
        }
        else
        {
            isSettingsOpen = false;
            settingsMenuUI.SetActive(false);
            mainMenuUI.SetActive(true);
        }
    }
    public void Credits()
    {
        if (!isCreditsOpen)
        {
            isCreditsOpen = true;
            creditsMenuUI.SetActive(true);
            mainMenuUI.SetActive(false);
        }
        else
        {
            isCreditsOpen = false;
            creditsMenuUI.SetActive(false);
            mainMenuUI.SetActive(true);
        }
    }
    public void StartLevel(int levelIndex)
    {
        Debug.Log("Starting level...");
        SceneManager.LoadSceneAsync(levelIndex+1);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
