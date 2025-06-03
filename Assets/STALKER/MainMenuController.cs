using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
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

    // public bool unloadCurrentScene = false;

    [Header("�����")]
    public AudioClip menuMusic; // Assign Assets/STALKER/sounds/wasteland/wasteland2.ogg � ����������
    public AudioSource audioSource;
    // public AudioMixerGroup musicMixerGroup; // �����������, ��� ���������� ����������

    private bool isMenuActive = false;
    private string currentGameScene;

    private void Awake()
    {
        // ������ ���� ������ � ����� ��������������
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        //PlayerPrefs.DeleteAll();

        // ������������� �����
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.clip = menuMusic;
            //if (musicMixerGroup != null)
            //{
            //}
        }

        // ���������, ���� �� ����������� ���� ��� ������ "����������"
        resumeGameButton.interactable = SaveSystem.HasSave();

        // ����������� ������ �� ������
        newGameButton.onClick.AddListener(StartNewGame);
        resumeGameButton.onClick.AddListener(ResumeGame);
        loadGameButton.onClick.AddListener(LoadGame);
        optionsButton.onClick.AddListener(ToggleOptions);
        exitButton.onClick.AddListener(ExitGame);

        // ������� ���� ���������
        SetMenuActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���������� �������� ������������� �����
        if (scene.name != "MainMenu_P")
        {
            currentGameScene = scene.name;
        }
    }

    private void Update()
    {
        // ��������� ������� Esc ������ ���� ���� �������
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

        // ����������/������������ UI �������� ����
        foreach (Transform child in transform)
        {
            if (child.gameObject != gameObject) // �� ������������ ��� ������
            {
                child.gameObject.SetActive(active);
            }
        }

        // ���������� �������
        if (active)
        {
            audioSource.Play();
            Time.timeScale = 0f; // ����� ����
        }
        else
        {
            audioSource.Stop();
            Time.timeScale = 1f; // ������������� ����

            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                optionsPanel.SetActive(false);
            }
        }

        // ����� ���� (���� �����)
        Time.timeScale = active ? 0f : 1f;
    }

    #region [START NEW GAME]
    // ����� ���� (��������� ����� � ����)
    private void StartNewGame()
    {
        SaveSystem.DeleteSave(); // ������� ���������� (�����������)
        SceneManager.LoadScene(newGameSceneName);
        SetMenuActive(false);
    }
    #endregion

    #region [RESUME CURRENT GAME]
    // ���������� ���� (��������� ��������� ����������)
    private void ResumeGame()
    {
        if (SaveSystem.HasSave())
        {
            SaveSystem.LoadGame(); // ��� ����� �������� ������
            SceneManager.LoadScene(SaveSystem.GetSavedSceneName());
        }
        SetMenuActive(false);
    }
    #endregion

    #region [LOAD GAME]
    // ���� �������� (��������, ����� �����)
    private void LoadGame()
    {
        Debug.Log("��������� ���� ��������...");
        Debug.Log("Opening load game menu...");
        // ����� ����� ������ ������ ������ ����������
        // ���� ������ ��������� ��������� ����������
        StartCoroutine(ResumeGameRoutine());
        // �������� ������ ��� ������ �����
        StartCoroutine(LoadSceneAdditive("GameScene"));
    }
    #endregion

    private IEnumerator LoadSceneAdditive(string sceneName)
    {
        // ��������� ������� ������� �����, ���� ��� ����
        if (!string.IsNullOrEmpty(currentGameScene))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentGameScene);
            while (!unloadOp.isDone)
            {
                yield return null;
            }
        }

        // ��������� ����� ����� ���������
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
        {
            yield return null;
        }

        // ������ ����� ����� ��������
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loadedScene); 
        currentGameScene = sceneName;

        // ��� ����� ���� ������� ����������
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
        // ����� ����� ������ ������ ������ ����������
        // ���� ������ ��������� ��������� ����������
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