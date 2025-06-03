
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DevConsole : MonoBehaviour
{
    public static DevConsole Instance { get; private set; }

    [Header("UI References")]
    public GameObject consolePanel;
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;
    public KeyCode toggleKey = KeyCode.BackQuote;

    [Header("Settings")]
    public bool adminModeRequired = true;
    public string adminPassword = "12345";
    public float fadeSpeed = 1f;

    private bool isConsoleVisible = false;
    private bool isAdmin = false;
    private List<string> commandHistory = new List<string>();
    private int historyIndex = -1;
    private CanvasGroup canvasGroup;
    private float targetAlpha = 0f;

    private string lastValidInput = ""; // Хранит последний валидный ввод

    private Dictionary<string, string> commands = new Dictionary<string, string>()
    {
        {"help", "Show all commands"},
        {"god", "Toggle god mode"},
        {"killall", "Destroy all enemies"},
        {"teleport", "Teleport to coordinates x y z"},
        {"admin", "Enter admin mode"}
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject); // (если нужно сохранить между сценами)

        // Настроим Canvas, чтобы рендерился поверх всего
        var canvas = consolePanel.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000; // Поверх всех обычных UI
        }
    }


    void Start()
    {
        // Добавляем CanvasGroup, если его нет
        if (!consolePanel.TryGetComponent(out canvasGroup))
        {
            canvasGroup = consolePanel.AddComponent<CanvasGroup>();
            Debug.Log("CanvasGroup added automatically");
        }

        canvasGroup = consolePanel.GetComponent<CanvasGroup>();
        consolePanel.SetActive(false);
        canvasGroup.alpha = 0;

        // Quake-style appearance
        consolePanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        outputText.color = Color.green;

        // удаеляем ` если остался с прошлого раза
        inputField.onValidateInput = (text, charIndex, addedChar) =>
        {
            return addedChar == '`' ? '\0' : addedChar;
        };

        // Настройка выравнивания текста
        if (outputText != null)
        {
            outputText.alignment = TextAlignmentOptions.Left;
            outputText.margin = new Vector4(10, 0, 0, 0); // Левый отступ 10px
        }
    }

    void Update()
    {
        // Плавное появление только если есть CanvasGroup
        if (canvasGroup != null)
        {
            // Плавное появление/исчезновение
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(toggleKey))
        {
            ToggleConsole();
        }

        if (!isConsoleVisible) return;

        HandleInput();
    }

    void ToggleConsole()
    {
        isConsoleVisible = !isConsoleVisible;
        targetAlpha = isConsoleVisible ? 1f : 0f;

        if (isConsoleVisible)
        {
            consolePanel.SetActive(true);
            inputField.text = lastValidInput; // Восстанавливаем ТОЛЬКО валидный текст
            inputField.ActivateInputField();
        }
        else
        {
            // Сохраняем только если текст соответствует критериям
            if (IsInputValid(inputField.text))
            {
                lastValidInput = inputField.text;
            }
            else
            {
                lastValidInput = "";
            }

            inputField.text = ""; // Всегда очищаем при закрытии
            historyIndex = -1;
        }
    }

    bool IsInputValid(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        // Регулярное выражение: только латиница, цифры и основные символы
        var regex = new Regex(@"^[a-zA-Z0-9_\-\+\=\.,!@#\$%\^\&\*\(\)\s]+$");
        return regex.IsMatch(input);
    }

    void HandleInput()
    {
        // Автоматическая очистка невалидных символов
        if (!IsInputValid(inputField.text))
        {
            inputField.text = Regex.Replace(inputField.text, @"[^a-zA-Z0-9_\-\+\=\.,!@#\$%\^\&\*\(\)\s]", "");
            inputField.caretPosition = inputField.text.Length;
        }

        // История команд
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (commandHistory.Count == 0) return;

            historyIndex = Mathf.Min(historyIndex + 1, commandHistory.Count - 1);
            inputField.text = commandHistory[commandHistory.Count - 1 - historyIndex];
            inputField.caretPosition = inputField.text.Length;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (historyIndex <= 0)
            {
                historyIndex = -1;
                inputField.text = "";
                return;
            }

            historyIndex--;
            inputField.text = commandHistory[commandHistory.Count - 1 - historyIndex];
            inputField.caretPosition = inputField.text.Length;
        }

        // Автодополнение при нажатии Tab
        if (Input.GetKeyDown(KeyCode.Tab) && !string.IsNullOrEmpty(inputField.text))
        {
            AutoComplete();
        }

        // Отправка команды
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ProcessCommand(inputField.text);
        }
    }

    void AutoComplete()
    {
        string partial = inputField.text.ToLower();
        var matches = commands.Keys.Where(cmd => cmd.StartsWith(partial)).ToList();

        if (matches.Count == 1)
        {
            inputField.text = matches[0];
            inputField.caretPosition = inputField.text.Length;
        }
        else if (matches.Count > 1)
        {
            AppendMessage($"Possible commands: {string.Join(", ", matches)}");
        }
    }

    void ProcessCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        commandHistory.Add(command);
        historyIndex = -1;

        // Вывод команды с левым отступом
        AppendMessage("> " + command.Trim());
        inputField.text = "";
        inputField.ActivateInputField();

        string[] parts = command.Split(' ');
        string cmd = parts[0].ToLower();

        switch (cmd)
        {
            case "help":
                // Красивый вывод списка команд
                AppendMessage("<b>Available commands:</b>");
                foreach (var kvp in commands)
                {
                    AppendMessage($" <b>{kvp.Key.PadRight(10)}</b> - {kvp.Value}");
                }
                break;

            case "admin":
                if (parts.Length > 1 && parts[1] == adminPassword)
                {
                    isAdmin = true;
                    AppendMessage("<color=green>Admin mode activated!</color>");
                    Debug.LogError("Admin mode activated!");
                }
                else
                {
                    AppendMessage("<color=red>Invalid password!</color>");
                }
                break;

            case "god":
                if (!adminModeRequired || isAdmin)
                {
                    AppendMessage("<color=yellow>God mode toggled</color>");
                }
                else
                {
                    AppendMessage("<color=red>Admin rights required!</color>");
                }
                break;
        }
    }

    void AppendMessage(string message)
    {
        // Добавляем отступ слева и перенос строки
        outputText.text += " " + message + "\n";

        // Автопрокрутка вниз
        Canvas.ForceUpdateCanvases();
        var scrollRect = outputText.GetComponentInParent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
