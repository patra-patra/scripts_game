using UnityEngine;

public class SimpleNPC : MonoBehaviour
{
    [Header("NPC Settings")]
    public string npcName = "NPC";
    public string npcId = "npc";

    [Header("Dialogue")]
    [TextArea(3, 5)]
    public string dialogueText = "Привет, путешественник!";

    [Header("Repeat Dialogue")]
    [TextArea(2, 3)]
    public string repeatDialogueText = "Мы уже говорили. Удачи в пути!";

    [Header("Quest Integration")]
    public string questId = "path_to_library";
    public string objectiveId = "talk_to_locals";
    public int progressAmount = 1;

    [Header("Interaction")]
    public float interactionRange = 3f;
    public KeyCode interactionKey = KeyCode.E;

    [Header("UI")]
    public GameObject interactionPrompt;

    private Transform player;
    private bool playerInRange = false;
    private bool hasInteracted = false;

    void Start()
    {
        // Находим игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Скрываем подсказку
        if (interactionPrompt)
            interactionPrompt.SetActive(false);

        // Настраиваем диалог для конкретного NPC
        SetupNPCData();
    }

    void SetupNPCData()
    {
        switch (npcId)
        {
            case "merchant":
                npcName = "Торговец Марк";
                dialogueText = "Библиотека-башня? Ах да, видел её на холме за тёмным лесом. Но будь осторожна, там водятся волки!";
                repeatDialogueText = "Помни - библиотека за тёмным лесом. Будь осторожна!";
                break;
            case "old_woman":
                npcName = "Бабушка Агата";
                dialogueText = "Дитя моё, библиотека старая и полна тайн. Мастер Элиас добрый, но место это... особенное. Возьми с собой фонарь!";
                repeatDialogueText = "Не забудь фонарь, дитя моё. В библиотеке бывает темно.";
                break;
            default:
                // Оставляем значения по умолчанию
                break;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Проверяем расстояние до игрока
        float distance = Vector3.Distance(transform.position, player.position);
        bool inRange = distance <= interactionRange;

        // Показываем/скрываем подсказку
        if (inRange != playerInRange)
        {
            playerInRange = inRange;
            if (interactionPrompt)
                interactionPrompt.SetActive(playerInRange);
        }

        // Обрабатываем взаимодействие (только если диалог не активен)
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            // Проверяем, не открыт ли уже диалог
            if (DialogueUI.Instance == null || !DialogueUI.Instance.IsActive())
            {
                Interact();
            }
        }
    }

    void Interact()
    {
        string textToShow;
        string hintText;

        if (!hasInteracted)
        {
            textToShow = dialogueText;
            hintText = "Кликните мышкой или нажмите Space";

            // Обновляем квест только при первом разговоре
            UpdateQuest();
            hasInteracted = true;
        }
        else
        {
            textToShow = repeatDialogueText;
            hintText = "Кликните для закрытия";
        }

        // Показываем диалог через UI
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.ShowDialogue(npcName, textToShow, hintText);
        }
        else
        {
            // Fallback на консоль если DialogueUI не найден
            Debug.Log($"=== {npcName} ===");
            Debug.Log(textToShow);
            Debug.LogWarning("DialogueUI не найден! Добавьте DialogueManager в сцену.");
        }

        // Триггерим событие диалога для квестов
        QuestEvents.TriggerNPCTalk(npcId, hasInteracted ? "repeat" : "first");
    }

    void UpdateQuest()
    {
        var questManager = FindObjectOfType<QuestObjectiveManager>();
        if (questManager != null)
        {
            questManager.UpdateCustomObjective(questId, objectiveId, progressAmount);
            Debug.Log($"Квест обновлен: {questId} - {objectiveId} (+{progressAmount})");
        }
        else
        {
            Debug.LogWarning("QuestObjectiveManager не найден!");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Показываем радиус взаимодействия
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}