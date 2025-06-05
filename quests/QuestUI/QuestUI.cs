using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private Transform questListParent;
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private Button toggleButton;

    [Header("Quest Details")]
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private Transform objectiveListParent;
    [SerializeField] private GameObject objectiveItemPrefab;
    [SerializeField] private Button startQuestButton;
    [SerializeField] private Button abandonQuestButton;

    private List<GameObject> questItems = new List<GameObject>();
    private List<GameObject> objectiveItems = new List<GameObject>();
    private Quest selectedQuest;
    private bool isUIVisible = false;

    private void Start()
    {
        // Подписка на события
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted += HandleQuestStarted;
            QuestManager.Instance.OnQuestCompleted += HandleQuestCompleted;
            QuestManager.Instance.OnQuestUnlocked += HandleQuestUnlocked;
            QuestManager.Instance.OnObjectiveCompleted += HandleObjectiveCompleted;
        }

        // Настройка кнопок
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleQuestUI);

        if (startQuestButton != null)
            startQuestButton.onClick.AddListener(StartSelectedQuest);

        if (abandonQuestButton != null)
            abandonQuestButton.onClick.AddListener(AbandonSelectedQuest);

        // Инициализация UI
        RefreshQuestList();
        questPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Отписка от событий
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted -= HandleQuestStarted;
            QuestManager.Instance.OnQuestCompleted -= HandleQuestCompleted;
            QuestManager.Instance.OnQuestUnlocked -= HandleQuestUnlocked;
            QuestManager.Instance.OnObjectiveCompleted -= HandleObjectiveCompleted;
        }
    }

    public void ToggleQuestUI()
    {
        isUIVisible = !isUIVisible;
        questPanel.SetActive(isUIVisible);

        if (isUIVisible)
        {
            RefreshQuestList();
        }
    }

    private void RefreshQuestList()
    {
        ClearQuestList();

        // Получаем все доступные и активные квесты
        var availableQuests = QuestManager.Instance.GetAvailableQuests();
        var activeQuests = QuestManager.Instance.GetActiveQuests();

        // Отображаем активные квесты
        foreach (var quest in activeQuests)
        {
            CreateQuestItem(quest, true);
        }

        // Отображаем доступные квесты
        foreach (var quest in availableQuests)
        {
            CreateQuestItem(quest, false);
        }
    }

    private void CreateQuestItem(Quest quest, bool isActive)
    {
        GameObject questItem = Instantiate(questItemPrefab, questListParent);
        questItems.Add(questItem);

        // Настройка отображения квеста
        var questItemUI = questItem.GetComponent<QuestItemUI>();
        if (questItemUI != null)
        {
            questItemUI.Setup(quest, isActive, SelectQuest);
        }
    }

    private void ClearQuestList()
    {
        foreach (var item in questItems)
        {
            if (item != null)
                Destroy(item);
        }
        questItems.Clear();
    }

    private void SelectQuest(Quest quest)
    {
        selectedQuest = quest;
        DisplayQuestDetails(quest);
    }

    private void DisplayQuestDetails(Quest quest)
    {
        if (quest == null) return;

        // Обновляем текст заголовка и описания
        if (questTitleText != null)
            questTitleText.text = quest.Title;

        if (questDescriptionText != null)
            questDescriptionText.text = quest.Description;

        // Отображаем цели
        DisplayObjectives(quest);

        // Настраиваем кнопки
        UpdateButtons(quest);
    }

    private void DisplayObjectives(Quest quest)
    {
        ClearObjectiveList();

        foreach (var objective in quest.Objectives)
        {
            GameObject objectiveItem = Instantiate(objectiveItemPrefab, objectiveListParent);
            objectiveItems.Add(objectiveItem);

            var objectiveUI = objectiveItem.GetComponent<ObjectiveItemUI>();
            if (objectiveUI != null)
            {
                objectiveUI.Setup(objective);
            }
        }
    }

    private void ClearObjectiveList()
    {
        foreach (var item in objectiveItems)
        {
            if (item != null)
                Destroy(item);
        }
        objectiveItems.Clear();
    }

    private void UpdateButtons(Quest quest)
    {
        if (startQuestButton != null)
        {
            startQuestButton.gameObject.SetActive(quest.Status == QuestStatus.Available);
        }

        if (abandonQuestButton != null)
        {
            abandonQuestButton.gameObject.SetActive(quest.Status == QuestStatus.Active);
        }
    }

    private void StartSelectedQuest()
    {
        if (selectedQuest != null && selectedQuest.Status == QuestStatus.Available)
        {
            QuestManager.Instance.StartQuest(selectedQuest.Id);
            RefreshQuestList();
            DisplayQuestDetails(selectedQuest);
        }
    }

    private void AbandonSelectedQuest()
    {
        if (selectedQuest != null && selectedQuest.Status == QuestStatus.Active)
        {
            QuestManager.Instance.FailQuest(selectedQuest.Id);
            RefreshQuestList();
            selectedQuest = null;
            ClearQuestDetails();
        }
    }

    private void ClearQuestDetails()
    {
        if (questTitleText != null)
            questTitleText.text = "";

        if (questDescriptionText != null)
            questDescriptionText.text = "";

        ClearObjectiveList();
        UpdateButtons(null);
    }

    // Обработчики событий
    private void HandleQuestStarted(Quest quest)
    {
        RefreshQuestList();
        if (selectedQuest != null && selectedQuest.Id == quest.Id)
        {
            DisplayQuestDetails(quest);
        }
    }

    private void HandleQuestCompleted(Quest quest)
    {
        RefreshQuestList();
        if (selectedQuest != null && selectedQuest.Id == quest.Id)
        {
            selectedQuest = null;
            ClearQuestDetails();
        }
    }

    private void HandleQuestUnlocked(Quest quest)
    {
        RefreshQuestList();
    }

    private void HandleObjectiveCompleted(Quest quest, QuestObjective objective)
    {
        if (selectedQuest != null && selectedQuest.Id == quest.Id)
        {
            DisplayObjectives(quest);
        }
    }
}