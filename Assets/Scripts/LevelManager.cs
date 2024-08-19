using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public string halilTest;

    public static LevelManager MainLevelSystem;
    private void Start()
    {
        if (MainLevelSystem == null)
        {
            MainLevelSystem = this;
            DontDestroyOnLoad(this);
        }
    }
    public void OnLevelSuccess()
    {
        SceneManager.LoadScene(halilTest);
    }
}
