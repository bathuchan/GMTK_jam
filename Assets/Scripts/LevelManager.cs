using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using EasyTransition;

public class LevelManager : MonoBehaviour
{
    
    [SerializeField]int currentLevelID=0;


    public static LevelManager MainLevelSystem;
    public TransitionSettings transition;
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
        //TransitionManager.Instance().Transition(SceneManager.GetSceneAt(currentLevelID).name,transition , 2f);
        Debug.Log("OnlevelSucces calisit");
    }
}
