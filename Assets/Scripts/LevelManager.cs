using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using EasyTransition;

public class LevelManager : MonoBehaviour
{
    
    [SerializeField]int currentLevelID;


    public static LevelManager MainLevelSystem;
    private void Start()
    {

        currentLevelID = SceneManager.GetActiveScene().buildIndex;
        if (MainLevelSystem == null)
        {
            MainLevelSystem = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
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
