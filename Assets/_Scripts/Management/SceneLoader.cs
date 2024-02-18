using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Singleton

    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public delegate void SceneLoaderEventHandler();
    public event SceneLoaderEventHandler OnSceneLoaded;

    [SerializeField] GameObject transitionPrefab;
    Animator transitionAnim = null;
    bool transitioning = false;

    [SerializeField] float transitionTime = 1f;

    private void OnEnable()
    {
        // Do this so random other instances don't start messing with the events
        if (Instance != this)
        {
            return;
        }
        SceneManager.sceneLoaded += SpawnTransitionPrefab;
    }
    private void OnDisable()
    {
        if (Instance != this)
        {
            return;
        }
        SceneManager.sceneLoaded -= SpawnTransitionPrefab;
    }

    public void StartGame()
    {
        GameManager.Instance.ResetGame();
        LoadScene("Game");
    }

    public void LoadMainMenu()
    {
        LoadScene("Main_Menu");
    }

    public void LoadScene(string sceneName)
    {
        if (transitioning)
        {
            return;
        }

        // The game should never be paused after loading a new scene
        Time.timeScale = 1;
        transitioning = true;
        StartCoroutine(AnimateTransition(sceneName));
    }

    private IEnumerator AnimateTransition(string sceneName)
    {
        transitionAnim.SetTrigger("Start");

        // Load the new scene asyncronously
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        yield return new WaitForSeconds(transitionTime);

        transitioning = false;
        async.allowSceneActivation = true;

        // Scene will only activate next frame
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        OnSceneLoaded?.Invoke();
    }

    private void SpawnTransitionPrefab(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
        {
            return;
        }

        GameObject g = Instantiate(transitionPrefab);
        transitionAnim = g.GetComponentInChildren<Animator>();
        if (transitionAnim == null)
        {
            Debug.LogError("There is no component of type Animator attached to any children of the transitionPrefab!");
        }
    }
}
