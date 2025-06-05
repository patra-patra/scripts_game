using UnityEngine;

public class SimpleNPC : MonoBehaviour
{
    [Header("NPC Settings")]
    public string npcName = "NPC";
    public string npcId = "npc";

    [Header("Dialogue")]
    [TextArea(3, 5)]
    public string dialogueText = "Привет, путешественник!";

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
                break;
            case "old_woman":
                npcName = "Бабушка Агата";
                dialogueText = "Дитя моё, библиотека старая и полна тайн. Мастер Элиас добрый, но место это... особенное. Возьми с собой светлые мысли.";
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

        // Обрабатываем взаимодействие
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
    }

    void Interact()
    {
        if (!hasInteracted)
        {
            Debug.Log($"=== {npcName} ===");
            Debug.Log(dialogueText);
            Debug.Log("Вы поговорили с " + npcName);

            // Обновляем квест
            UpdateQuest();
            hasInteracted = true;
        }
        else
        {
            Debug.Log($"{npcName}: Мы уже говорили. Удачи в пути!");
        }
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