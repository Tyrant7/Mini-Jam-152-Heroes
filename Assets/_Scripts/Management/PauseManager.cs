using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public static bool Paused { get; private set; } = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused = !Paused;
            ShowHidePauseMenu();
        }
        Time.timeScale = Paused ? 0 : 1;
    }

    private void ShowHidePauseMenu()
    {
        AudioManager.PlayButtonClick();
        pauseMenu.SetActive(Paused);
    }

    public void Resume()
    {
        Paused = false;
        ShowHidePauseMenu();
    }

    public void ReturnToMenu()
    {
        Paused = false;
        ShowHidePauseMenu();
        GameManager.Instance.ExitToMainMenu();
        SceneLoader.Instance.LoadMainMenu();
    }
}
