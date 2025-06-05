using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI hintText;
    public Button closeButton;

    [Header("Click Settings")]
    public bool closeOnClick = true;
    public bool closeOnClickOutside = true;

    [Header("Hint Settings")]
    public string hintMessage = "Кликните для продолжения";
    public Color hintColor = Color.gray;

    private static DialogueUI instance;
    public static DialogueUI Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Скрываем панель при старте
        dialoguePanel.SetActive(false);

        // Настраиваем кнопку закрытия
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDialogue);

        // Настраиваем подсказку
        SetupHintText();
    }

    void SetupHintText()
    {
        if (hintText != null)
        {
            hintText.color = hintColor;
            hintText.fontSize = 12;
            hintText.alpha = 0.7f;
        }
    }

    public void ShowDialogue(string npcName, string dialogue)
    {
        // Устанавливаем тексты
        if (npcNameText != null)
            npcNameText.text = npcName;

        if (dialogueText != null)
            dialogueText.text = dialogue;

        // Показываем подсказку
        if (hintText != null)
            hintText.text = hintMessage;

        // Показываем панель
        dialoguePanel.SetActive(true);

        // Ставим игру на паузу
        Time.timeScale = 0f;

        Debug.Log($"Показан диалог с {npcName}: {dialogue}");
    }

    public void CloseDialogue()
    {
        // Скрываем панель
        dialoguePanel.SetActive(false);

        // Снимаем с паузы
        Time.timeScale = 1f;

        Debug.Log("Диалог закрыт");
    }

    public bool IsActive()
    {
        return dialoguePanel != null && dialoguePanel.activeInHierarchy;
    }

    void Update()
    {
        // Проверяем, активен ли диалог
        if (!IsActive()) return;

        // Закрытие на клик мыши
        if (closeOnClick && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            // Если включено закрытие по клику вне панели
            if (closeOnClickOutside)
            {
                Vector2 mousePos = Input.mousePosition;
                RectTransform panelRect = dialoguePanel.GetComponent<RectTransform>();

                // Проверяем, кликнули ли вне панели
                if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePos, Camera.main))
                {
                    CloseDialogue();
                }
                else
                {
                    // Кликнули внутри панели - тоже закрываем
                    CloseDialogue();
                }
            }
            else
            {
                // Закрываем на любой клик
                CloseDialogue();
            }
        }

        // Закрытие на клавиши
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialogue();
        }
    }

    // Метод для изменения текста подсказки
    public void SetHintText(string newHint)
    {
        hintMessage = newHint;
        if (hintText != null && IsActive())
            hintText.text = hintMessage;
    }

    // Метод для показа диалога с кастомной подсказкой
    public void ShowDialogue(string npcName, string dialogue, string customHint)
    {
        SetHintText(customHint);
        ShowDialogue(npcName, dialogue);
    }
}