using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoader.Instance.StartGame();
    }

    public void StartTutorial()
    {
        Debug.Log("starting tutorial");
        SceneLoader.Instance.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        if (Application.isEditor)
        {
            return;
        }
        Application.Quit();
    }
}
