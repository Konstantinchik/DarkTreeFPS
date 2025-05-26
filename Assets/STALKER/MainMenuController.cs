using System.Collections;
using UnityEngine;
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

    public bool unloadCurrentScene = false;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();

        // Проверяем, есть ли сохраненная игра для кнопки "Продолжить"
        resumeGameButton.interactable = SaveSystem.HasSave();

        // Подписываем методы на кнопки
        newGameButton.onClick.AddListener(StartNewGame);
        resumeGameButton.onClick.AddListener(ResumeGame);
        loadGameButton.onClick.AddListener(LoadGame);
        optionsButton.onClick.AddListener(ToggleOptions);
        exitButton.onClick.AddListener(ExitGame);
    }

    // Новая игра (загружает сцену с нуля)
    private void StartNewGame()
    {
        SaveSystem.DeleteSave(); // Очищаем сохранения (опционально)
        SceneManager.LoadScene(newGameSceneName);
    }

    // Продолжить игру (загружает последнее сохранение)
    private void ResumeGame()
    {
        if (SaveSystem.HasSave())
        {
            SaveSystem.LoadGame(); // Ваш метод загрузки данных
            SceneManager.LoadScene(SaveSystem.GetSavedSceneName());
        }
    }

    // Меню загрузки (например, выбор слота)
    private void LoadGame()
    {
        Debug.Log("Открываем меню загрузки...");
        // Дописать логику для выбора слота
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {

        AsyncOperation loadOp = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByName("GameScene");
        SceneManager.SetActiveScene(newScene);

        if (unloadCurrentScene)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != "GameScene")
                SceneManager.UnloadSceneAsync(currentScene);
        }

     
    }

    // Вкл/выкл панель настроек
    private void ToggleOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }

    // Выход из игры
    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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