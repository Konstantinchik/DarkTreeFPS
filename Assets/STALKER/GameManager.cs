using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsPaused { get; private set; }

    private const string MainMenuSceneName = "MainMenu_P";

    // Список загруженных аддитивных сцен
    public List<string> LoadedGameScenes { get; private set; } = new List<string>();

    public bool IsInGame => LoadedGameScenes.Count > 0;
    public bool IsInMainMenu => !IsInGame;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MainMenuSceneName)
            return; // не считаем основное меню как игровую сцену

        if (!LoadedGameScenes.Contains(scene.name))
        {
            LoadedGameScenes.Add(scene.name);
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (LoadedGameScenes.Contains(scene.name))
        {
            LoadedGameScenes.Remove(scene.name);
        }
    }

    public void SetPaused(bool isPaused)
    {
        IsPaused = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void TogglePause()
    {
        SetPaused(!IsPaused);
    }


    public void UnloadAllGameScenes()
    {
        foreach (var sceneName in new List<string>(LoadedGameScenes))
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        LoadedGameScenes.Clear();
    }

    public void AddGameScene(string sceneName)
    {
        if (!LoadedGameScenes.Contains(sceneName))
        {
            LoadedGameScenes.Add(sceneName);
        }
    }

    public void RemoveGameScene(string sceneName)
    {
        if (LoadedGameScenes.Contains(sceneName))
        {
            LoadedGameScenes.Remove(sceneName);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}