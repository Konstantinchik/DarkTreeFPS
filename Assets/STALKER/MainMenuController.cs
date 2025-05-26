using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("������")]
    public Button newGameButton;
    public Button resumeGameButton;
    public Button loadGameButton;
    public Button optionsButton;
    public Button exitButton;

    [Header("���������")]
    public string newGameSceneName = "GameScene"; // ��� ����� ��� "����� ����"
    public GameObject optionsPanel; // ������ �������� (UI Panel)

    public bool unloadCurrentScene = false;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();

        // ���������, ���� �� ����������� ���� ��� ������ "����������"
        resumeGameButton.interactable = SaveSystem.HasSave();

        // ����������� ������ �� ������
        newGameButton.onClick.AddListener(StartNewGame);
        resumeGameButton.onClick.AddListener(ResumeGame);
        loadGameButton.onClick.AddListener(LoadGame);
        optionsButton.onClick.AddListener(ToggleOptions);
        exitButton.onClick.AddListener(ExitGame);
    }

    // ����� ���� (��������� ����� � ����)
    private void StartNewGame()
    {
        SaveSystem.DeleteSave(); // ������� ���������� (�����������)
        SceneManager.LoadScene(newGameSceneName);
    }

    // ���������� ���� (��������� ��������� ����������)
    private void ResumeGame()
    {
        if (SaveSystem.HasSave())
        {
            SaveSystem.LoadGame(); // ��� ����� �������� ������
            SceneManager.LoadScene(SaveSystem.GetSavedSceneName());
        }
    }

    // ���� �������� (��������, ����� �����)
    private void LoadGame()
    {
        Debug.Log("��������� ���� ��������...");
        // �������� ������ ��� ������ �����
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

    // ���/���� ������ ��������
    private void ToggleOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }

    // ����� �� ����
    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}


// ��������
public static class SaveSystem
{
    private const string SAVE_KEY = "GameSave";

    // ���������, ���� �� ����������
    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    // ��������� ������
    public static void LoadGame()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY);
        // ������������� json � ������� ������
    }

    // �������� ��� ����������� �����
    public static string GetSavedSceneName()
    {
        return PlayerPrefs.GetString("LastScene", "GameScene");
    }

    // ������� ����������
    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
    }
}