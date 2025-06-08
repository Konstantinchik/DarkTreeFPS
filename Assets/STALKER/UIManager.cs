using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuUIPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject hudPanel;

    private void OnEnable()
    {
        GameManager.OnGameEntered += HandleGameEntered;
        GameManager.OnReturnedToMenu += HandleReturnedToMenu;
        GameManager.OnPaused += HandlePaused;
        GameManager.OnResumed += HandleResumed;
    }

    private void OnDisable()
    {
        GameManager.OnGameEntered -= HandleGameEntered;
        GameManager.OnReturnedToMenu -= HandleReturnedToMenu;
        GameManager.OnPaused -= HandlePaused;
        GameManager.OnResumed -= HandleResumed;
    }

    private void Start()
    {
        // Начальное состояние — отображается только главное меню
        ShowMainMenu();
    }

    private void HandleGameEntered()
    {
        HideAll();
        ShowHUD();
    }

    private void HandleReturnedToMenu()
    {
        HideAll();
        ShowMainMenu();
    }

    private void HandlePaused()
    {
        ShowPauseMenu();
    }

    private void HandleResumed()
    {
        HidePauseMenu();
    }

    private void ShowMainMenu()
    {
        mainMenuUIPanel?.SetActive(true);
    }

    private void ShowHUD()
    {
        if (hudPanel != null) hudPanel?.SetActive(true);
    }

    private void ShowPauseMenu()
    {
        pauseMenuPanel?.SetActive(true);
    }

    private void HidePauseMenu()
    {
        pauseMenuPanel?.SetActive(false);
    }

    private void HideAll()
    {
        mainMenuUIPanel?.SetActive(false);
        pauseMenuPanel?.SetActive(false);
        if (hudPanel != null) hudPanel?.SetActive(false);
    }
}
