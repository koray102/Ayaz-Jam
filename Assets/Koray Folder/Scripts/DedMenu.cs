using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DedMenu : MonoBehaviour
{
    public void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(0);
    }

    
    public void ExitGame()
    {
        Application.Quit();
    }
}
