using DarkTreeFPS;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }

    /* ---------- UI ---------- */
    [Header("Кнопки")]
    public Button newGameButton;
    public Button resumeGameButton;
    public Button loadGameButton;
    public Button optionsButton;
    public Button exitButton;

    [Header("Панели")]
    public GameObject optionsPanel;          // вкладки настроек
    public GameObject pauseMenuPanel;        // если есть отдельная панель паузы

    /* ---------- Настройки ---------- */
    [Header("Настройки")]
    public string newGameSceneName = "GameScene"; // Имя сцены для "Новой игры"
    

    /* ---------- Внутреннее ---------- */
    public bool IsActive => isMenuActive;

    private bool isMenuActive = true;
    private string currentGameScene;

    #region Unity Lifecycle
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
    }

    private void Start()
    {

        // PlayerPrefs.DeleteAll();

        // Подписываем методы на кнопки
        newGameButton.onClick.AddListener(StartNewGame);
        resumeGameButton.onClick.AddListener(ResumeGame);
        loadGameButton.onClick.AddListener(LoadGame);
        optionsButton.onClick.AddListener(ToggleOptions);
        exitButton.onClick.AddListener(ExitGame);

        resumeGameButton.interactable = SaveSystem.HasSave();

    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }
    #endregion

    /* ---------- Scene Events ---------- */
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        /* Если это не MainMenu_P, запоминаем как «текущую игровую» */
        if (scene.name != "MainMenu_P")
        {
            currentGameScene = scene.name;
            /* меню должно быть скрыто, игра на паузу — отключена */
            SetMenuActive(false);
        }
    }


    /* ---------- Меню On/Off ---------- */
    private void SetMenuActive(bool active)
    {
        isMenuActive = active;

        /* Активируем / прячем все дочерние UI-элементы */
        foreach (Transform child in transform)
        {
            if (child.gameObject != gameObject)
                child.gameObject.SetActive(active);
        }

        /* Управление музыкой и временем */
        if (active)
        {
            SoundController.Instance.PlayMenuMusic();
            Time.timeScale = 0f;
        }
        else
        {
            SoundController.Instance.StopMenuMusic();
            Time.timeScale = 1f;
            if (optionsPanel != null) optionsPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        }
    }

    /* ---------- API для Pause-меню ---------- */
    public void ReturnToMainMenu()          // вызывается кнопкой "Main Menu"
    {
        /* выключаем паузу */
        Time.timeScale = 0f;
        /* выгружаем текущую игровую сцену, если она есть */
        if (!string.IsNullOrEmpty(currentGameScene))
            StartCoroutine(UnloadCurrentGameScene());

        /* показываем главное меню */
        SetMenuActive(true);
    }

    private IEnumerator UnloadCurrentGameScene()
    {
        AsyncOperation op = SceneManager.UnloadSceneAsync(currentGameScene);
        while (!op.isDone) yield return null;
        currentGameScene = null;
    }

    /* ---------- Кнопки главного меню ---------- */
    private void StartNewGame()
    {
        StartCoroutine(LoadSceneAdditive(newGameSceneName));
    }

    private void ResumeGame()
    {
        if (SaveSystem.HasSave())
        {
            /* тут может быть своя логика загрузки прогресса */
            StartCoroutine(LoadSceneAdditive(newGameSceneName));
        }
    }

    private void LoadGame()
    {
        /* заглушка под выбор слота и т.д. */
        StartCoroutine(LoadSceneAdditive(newGameSceneName));
    }

    private void ToggleOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /* ---------- Additive Loader ---------- */
    private IEnumerator LoadSceneAdditive(string sceneName)
    {
        /* выгружаем предыдущую игровую, если есть */
        if (!string.IsNullOrEmpty(currentGameScene))
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(currentGameScene);
            while (!unload.isDone) yield return null;
        }

        /* загружаем новую */
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!load.isDone) yield return null;

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loadedScene);
        currentGameScene = sceneName;

        /* после загрузки — убираем меню и начинаем игру */
        SetMenuActive(false);
    }
}


// КЛАСС - ЗАГЛУШКА         SAVE SYSTEM 
public static class SaveSystem
{
    private const string SAVE_KEY = "GameSave";

    // Проверяем, есть ли сохранение
    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    // Загружаем данные
    public static void LoadGame()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY);
        // Десериализуем json в игровые данные
    }
}