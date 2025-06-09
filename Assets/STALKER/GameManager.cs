using DarkTreeFPS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // СПИСОК МЕТОДОВ
    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // private void OnSceneUnloaded(Scene scene)
    // private void UpdateMainMenuStatus()
    // public void SetPaused(bool isPaused)
    // public void TogglePause()
    // void SetMainMenu()
    // void QuitCurrentMenu()
    // public void UnloadAllGameScenes()
    // public void AddGameScene(string sceneName)
    // public void RemoveGameScene(string sceneName)

    public static GameManager Instance { get; private set; }

    public bool IsPaused { get; private set; }
    public bool IsInMainMenu { get; private set; }

    // будем вызывать их в нужных местах (UpdateMainMenuStatus, SetPaused и т.п.).
    // События будут статическими, чтобы на них можно было подписываться без ссылки на GameManager.Instance.
    public static event Action OnGameEntered;
    public static event Action OnReturnedToMenu;
    public static event Action OnPaused;
    public static event Action OnResumed;
    public static event Action<GameObject> OnPlayerSpawned;

    private GameObject currentPlayer; // здесь ссылка на префаб игрока в сцене

    private const string MainMenuSceneName = "MainMenu_P";

    // Список загруженных аддитивных сцен
    public List<string> LoadedGameScenes { get; private set; } = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        IsInMainMenu = true; // Когда загружается MainMenu_P - это MainMenu

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        // UpdateMainMenuStatus будет отслеживать изменение состояния и вызывать нужное событие.
        UpdateMainMenuStatus();
    }

    #region Подписка и отписка на Спавн игрока. Обработка спавна
    private void OnEnable()
    {
        OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        OnPlayerSpawned -= HandlePlayerSpawned;
    }

    // Вызов события когда Player готов
    public static void NotifyPlayerSpawned(GameObject player)
    {
        OnPlayerSpawned?.Invoke(player);
    }

    private void HandlePlayerSpawned(GameObject player)
    {
        currentPlayer = player;
        Debug.Log($"[GameManager] Player spawned: {player.name}");

        // Можешь здесь получить доступ к нужным компонентам:
        var stats = player.GetComponent<PlayerStats>();
        var cam = player.GetComponentInChildren<Camera>();

        // Или передать их в другие менеджеры (UIManager, AudioManager и т.д.)
    }
    #endregion

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            Instance = null;
        }
    }

    /// <summary>
    /// Когда загружаем аддитивную сцену, добавляем её в список
    /// </summary>
    /// <param name="scene"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != MainMenuSceneName && !LoadedGameScenes.Contains(scene.name))
        {
            LoadedGameScenes.Add(scene.name);
        }

        UpdateMainMenuStatus();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (LoadedGameScenes.Contains(scene.name))
        {
            LoadedGameScenes.Remove(scene.name);
        }

        UpdateMainMenuStatus();
    }

    private void UpdateMainMenuStatus()
    {
        bool newIsInMainMenu = LoadedGameScenes.Count == 0;

        if (IsInMainMenu != newIsInMainMenu)
        {
            IsInMainMenu = newIsInMainMenu;

            if (IsInMainMenu)
            {
                Debug.Log("[GameManager] Returned to Main Menu.");
                OnReturnedToMenu?.Invoke();
            }
            else
            {
                Debug.Log("[GameManager] Game Entered.");
                OnGameEntered?.Invoke();
            }
        }
    }

    /// <summary>
    /// Вызывается при нажатии на Esc
    /// </summary>
    /// <param name="isPaused"></param>
    public void SetPaused(bool isPaused)
    {
        if (IsInMainMenu) return;

        if (IsPaused != isPaused)
        {
            IsPaused = isPaused;
            Time.timeScale = isPaused ? 0f : 1f;

            if (isPaused)
            {
                Debug.Log("[GameManager] Game Paused.");
                OnPaused?.Invoke();
            }
            else
            {
                Debug.Log("[GameManager] Game Resumed.");
                OnResumed?.Invoke();
            }
        }
    }

    /// <summary>
    /// Переключаем пауза-не пауза только если мы не в главном меню
    /// </summary>
    public void TogglePause()
    {
        if (!IsInMainMenu)
            SetPaused(!IsPaused);
    }

    /// <summary>
    /// Переключаем главное меню - игра
    /// </summary>
    public void ToggleMainMenu()
    {
        if (IsInMainMenu)
            QuitCurrentMenu();
        else
            SetMainMenu();
    }

    void SetMainMenu()
    {
        Debug.Log("[GameManager] Switching to Main Menu...");
        UnloadAllGameScenes();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(MainMenuSceneName));
        UpdateMainMenuStatus();
    }

    void QuitCurrentMenu()
    {
        Debug.Log("[GameManager] Leaving Main Menu...");
        // Например, при старте игры — логика загрузки игровых сцен будет отдельно
        UpdateMainMenuStatus();
    }

    /// <summary>
    /// Выгружаются все аддитивно загруженные сцены
    /// </summary>
    public void UnloadAllGameScenes()
    {
        foreach (var sceneName in new List<string>(LoadedGameScenes))
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        LoadedGameScenes.Clear();
        UpdateMainMenuStatus();
    }

    /// <summary>
    /// Когда загружаем локацию, добавляем её в список
    /// </summary>
    /// <param name="sceneName"></param>
    public void AddGameScene(string sceneName)
    {
        if (!LoadedGameScenes.Contains(sceneName))
        {
            LoadedGameScenes.Add(sceneName);
            UpdateMainMenuStatus();
        }
    }

    /// <summary>
    /// Когда выгружаем локацию, удаляем её из списка
    /// </summary>
    /// <param name="sceneName"></param>
    public void RemoveGameScene(string sceneName)
    {
        if (LoadedGameScenes.Contains(sceneName))
        {
            LoadedGameScenes.Remove(sceneName);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endif
    }
}