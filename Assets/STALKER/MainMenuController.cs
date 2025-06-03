using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Кнопки")]
    public Button newGameButton;
    public Button resumeGameButton;
    public Button loadGameButton;
    public Button optionsButton;
    public Button exitButton;

    [Header("Настройки")]
    public string newGameSceneName = "GameScene"; // Имя сцены для "Новой игры"
    public GameObject optionsPanel; // Панель настроек (UI Panel)

    // public bool unloadCurrentScene = false;

    [Header("Аудио")]
    public AudioClip menuMusic; // Assign Assets/STALKER/sounds/wasteland/wasteland2.ogg в инспекторе
    public AudioSource audioSource;
    // public AudioMixerGroup musicMixerGroup; // Опционально, для управления громкостью

    private bool isMenuActive = false;
    private string currentGameScene;

    private void Awake()
    {
        // Делаем этот объект и сцену персистентными
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        //PlayerPrefs.DeleteAll();

        // Инициализация аудио
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.clip = menuMusic;
            //if (musicMixerGroup != null)
            //{
            //}
        }

        // Проверяем, есть ли сохраненная игра для кнопки "Продолжить"
        resumeGameButton.interactable = SaveSystem.HasSave();

        // Подписываем методы на кнопки
        newGameButton.onClick.AddListener(StartNewGame);
        resumeGameButton.onClick.AddListener(ResumeGame);
        loadGameButton.onClick.AddListener(LoadGame);
        optionsButton.onClick.AddListener(ToggleOptions);
        exitButton.onClick.AddListener(ExitGame);

        // Сначала меню неактивно
        SetMenuActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Игнорируем загрузку персистентной сцены
        if (scene.name != "MainMenu_P")
        {
            currentGameScene = scene.name;
        }
    }

    private void Update()
    {
        // Проверяем нажатие Esc только если меню активно
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        SetMenuActive(!isMenuActive);
    }

    private void SetMenuActive(bool active)
    {
        isMenuActive = active;

        // Активируем/деактивируем UI элементы меню
        foreach (Transform child in transform)
        {
            if (child.gameObject != gameObject) // Не деактивируем сам объект
            {
                child.gameObject.SetActive(active);
            }
        }

        // Управление музыкой
        if (active)
        {
            audioSource.Play();
            Time.timeScale = 0f; // Пауза игры
        }
        else
        {
            audioSource.Stop();
            Time.timeScale = 1f; // Возобновление игры

            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                optionsPanel.SetActive(false);
            }
        }

        // Пауза игры (если нужно)
        Time.timeScale = active ? 0f : 1f;
    }

    #region [START NEW GAME]
    // Новая игра (загружает сцену с нуля)
    private void StartNewGame()
    {
        SaveSystem.DeleteSave(); // Очищаем сохранения (опционально)
        SceneManager.LoadScene(newGameSceneName);
        SetMenuActive(false);
    }
    #endregion

    #region [RESUME CURRENT GAME]
    // Продолжить игру (загружает последнее сохранение)
    private void ResumeGame()
    {
        if (SaveSystem.HasSave())
        {
            SaveSystem.LoadGame(); // Ваш метод загрузки данных
            SceneManager.LoadScene(SaveSystem.GetSavedSceneName());
        }
        SetMenuActive(false);
    }
    #endregion

    #region [LOAD GAME]
    // Меню загрузки (например, выбор слота)
    private void LoadGame()
    {
        Debug.Log("Открываем меню загрузки...");
        Debug.Log("Opening load game menu...");
        // Здесь будет логика выбора слотов сохранений
        // Пока просто загружаем последнее сохранение
        StartCoroutine(ResumeGameRoutine());
        // Дописать логику для выбора слота
        StartCoroutine(LoadSceneAdditive("GameScene"));
    }
    #endregion

    private IEnumerator LoadSceneAdditive(string sceneName)
    {
        // Выгружаем текущую игровую сцену, если она есть
        if (!string.IsNullOrEmpty(currentGameScene))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentGameScene);
            while (!unloadOp.isDone)
            {
                yield return null;
            }
        }

        // Загружаем новую сцену аддитивно
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
        {
            yield return null;
        }

        // Делаем новую сцену активной
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loadedScene); 
        currentGameScene = sceneName;

        // Для новой игры очищаем сохранения
        if (sceneName == newGameSceneName)
        {
            SaveSystem.DeleteSave();
        }

        SetMenuActive(false);
    }

    private IEnumerator ResumeGameRoutine()
    {
        if (SaveSystem.HasSave())
        {
            string savedSceneName = SaveSystem.GetSavedSceneName();
            yield return StartCoroutine(LoadSceneAdditive(savedSceneName));
            SaveSystem.LoadGame();
        }
    }

    /*
    private void LoadGame()
    {
        Debug.Log("Opening load game menu...");
        // Здесь будет логика выбора слотов сохранений
        // Пока просто загружаем последнее сохранение
        StartCoroutine(ResumeGameRoutine());
    }
    */

    private void ToggleOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }

    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}


// ЗАГЛУШКА
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

    // Получаем имя сохраненной сцены
    public static string GetSavedSceneName()
    {
        return PlayerPrefs.GetString("LastScene", "GameScene");
    }

    // Удаляем сохранение
    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
    }
}