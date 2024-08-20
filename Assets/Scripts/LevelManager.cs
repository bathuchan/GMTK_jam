using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    
    [SerializeField]int currentLevelID=0;


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
        currentLevelID++;
        SceneManager.LoadScene(currentLevelID);
        
    }
}
