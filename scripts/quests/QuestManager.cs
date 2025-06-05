using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private bool _debugMode = false;

    private IQuestRepository _questRepository;
    private List<Quest> _activeQuests;
    private QuestDependencyResolver _dependencyResolver;
    private QuestObjectiveManager _objectiveManager;

    // События
    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestCompleted;
    public event Action<Quest> OnQuestFailed;
    public event Action<Quest> OnQuestUnlocked;
    public event Action<Quest, QuestObjective> OnObjectiveCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        _questRepository = new QuestRepository();
        _activeQuests = new List<Quest>();
        _dependencyResolver = new QuestDependencyResolver(_questRepository);
        _objectiveManager = GetComponent<QuestObjectiveManager>();

        if (_objectiveManager == null)
        {
            _objectiveManager = gameObject.AddComponent<QuestObjectiveManager>();
        }

        // Подписка на события целей квестов
        _objectiveManager.OnObjectiveCompleted += HandleObjectiveCompleted;
        _objectiveManager.OnAllObjectivesCompleted += HandleAllObjectivesCompleted;

        LoadActiveQuests();
        UpdateQuestAvailability();

        LogDebug("QuestManager initialized");
    }

    private void Start()
    {
        // Подписка на игровые события
        SubscribeToGameEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromGameEvents();

        if (_objectiveManager != null)
        {
            _objectiveManager.OnObjectiveCompleted -= HandleObjectiveCompleted;
            _objectiveManager.OnAllObjectivesCompleted -= HandleAllObjectivesCompleted;
        }
    }

    public bool StartQuest(string questId)
    {
        var quest = _questRepository.GetQuestById(questId);
        if (quest == null)
        {
            LogDebug($"Quest with ID {questId} not found");
            return false;
        }

        if (!CanStartQuest(quest))
        {
            LogDebug($"Cannot start quest {questId}. Status: {quest.Status}");
            return false;
        }

        quest.Status = QuestStatus.Active;
        _activeQuests.Add(quest);
        _questRepository.SaveQuestProgress(quest);

        OnQuestStarted?.Invoke(quest);
        LogDebug($"Started quest: {quest.Title}");

        return true;
    }

    public void CompleteQuest(string questId)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.Id == questId);
        if (quest == null)
        {
            LogDebug($"Active quest with ID {questId} not found");
            return;
        }

        quest.Status = QuestStatus.Completed;
        quest.CompletedAt = DateTime.UtcNow;
        _activeQuests.Remove(quest);
        _questRepository.SaveQuestProgress(quest);

        OnQuestCompleted?.Invoke(quest);
        LogDebug($"Completed quest: {quest.Title}");

        // Выдача награды
        if (quest.Reward != null && quest.Reward.HasRewards())
        {
            GiveReward(quest.Reward);
        }

        // Проверяем, разблокировались ли новые квесты
        UpdateQuestAvailability();
    }

    public void FailQuest(string questId)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.Id == questId);
        if (quest == null) return;

        quest.Status = QuestStatus.Failed;
        _activeQuests.Remove(quest);
        _questRepository.SaveQuestProgress(quest);

        OnQuestFailed?.Invoke(quest);
        LogDebug($"Failed quest: {quest.Title}");
    }

    public Quest GetActiveQuest(string questId)
    {
        return _activeQuests.FirstOrDefault(q => q.Id == questId);
    }

    public List<Quest> GetActiveQuests()
    {
        return _activeQuests.ToList();
    }

    public List<Quest> GetAvailableQuests()
    {
        return _questRepository.GetAvailableQuests();
    }

    public List<Quest> GetCompletedQuests()
    {
        return _questRepository.GetCompletedQuests();
    }

    public bool IsQuestActive(string questId)
    {
        return _activeQuests.Any(q => q.Id == questId);
    }

    public bool IsQuestCompleted(string questId)
    {
        var quest = _questRepository.GetQuestById(questId);
        return quest != null && quest.Status == QuestStatus.Completed;
    }

    private bool CanStartQuest(Quest quest)
    {
        return quest.Status == QuestStatus.Available &&
               _dependencyResolver.ArePrerequisitesMet(quest) &&
               !_activeQuests.Any(q => q.Id == quest.Id);
    }

    private void UpdateQuestAvailability()
    {
        var allQuests = _questRepository.GetAllQuests();
        foreach (var quest in allQuests)
        {
            if (quest.Status == QuestStatus.Locked &&
                _dependencyResolver.ArePrerequisitesMet(quest))
            {
                quest.Status = QuestStatus.Available;
                _questRepository.SaveQuestProgress(quest);
                OnQuestUnlocked?.Invoke(quest);
                LogDebug($"Unlocked quest: {quest.Title}");
            }
        }
    }

    private void LoadActiveQuests()
    {
        var activeQuests = _questRepository.GetActiveQuests();
        _activeQuests = activeQuests.ToList();
        LogDebug($"Loaded {_activeQuests.Count} active quests");
    }

    private void HandleObjectiveCompleted(Quest quest, QuestObjective objective)
    {
        OnObjectiveCompleted?.Invoke(quest, objective);
        LogDebug($"Objective completed: {objective.Description} in quest {quest.Title}");
    }

    private void HandleAllObjectivesCompleted(Quest quest)
    {
        CompleteQuest(quest.Id);
    }

    private void GiveReward(QuestReward reward)
    {
        // Здесь должна быть интеграция с системами игры
        LogDebug($"Giving reward: {reward.Experience} XP, {reward.Gold} Gold, {reward.Items.Count} Items");

        // Пример интеграции:
        // PlayerManager.Instance.AddExperience(reward.Experience);
        // PlayerManager.Instance.AddGold(reward.Gold);
        // foreach (var item in reward.Items)
        // {
        //     InventoryManager.Instance.AddItem(item.ItemId, item.Quantity);
        // }
    }

    private void SubscribeToGameEvents()
    {
        QuestEvents.OnItemCollected += HandleItemCollected;
        QuestEvents.OnEnemyKilled += HandleEnemyKilled;
        QuestEvents.OnNPCTalkedTo += HandleNPCTalkedTo;
        QuestEvents.OnLocationReached += HandleLocationReached;
    }

    private void UnsubscribeFromGameEvents()
    {
        QuestEvents.OnItemCollected -= HandleItemCollected;
        QuestEvents.OnEnemyKilled -= HandleEnemyKilled;
        QuestEvents.OnNPCTalkedTo -= HandleNPCTalkedTo;
        QuestEvents.OnLocationReached -= HandleLocationReached;
    }

    private void HandleItemCollected(string itemId)
    {
        _objectiveManager?.UpdateObjectivesForItemCollection(itemId);
    }

    private void HandleEnemyKilled(string enemyType)
    {
        _objectiveManager?.UpdateObjectivesForEnemyKill(enemyType);
    }

    private void HandleNPCTalkedTo(string npcId, string dialogueId)
    {
        _objectiveManager?.UpdateObjectivesForNPCTalk(npcId, dialogueId);
    }

    private void HandleLocationReached(Vector3 location)
    {
        _objectiveManager?.UpdateObjectivesForLocationReached(location);
    }

    private void LogDebug(string message)
    {
        if (_debugMode)
        {
            Debug.Log($"[QuestManager] {message}");
        }
    }

    // Методы для отладки и тестирования
    public void ResetAllQuests()
    {
        _activeQuests.Clear();
        _questRepository.ResetAllQuests();
        UpdateQuestAvailability();
        LogDebug("All quests reset");
    }

    public void CompleteAllActiveQuests()
    {
        var questsToComplete = _activeQuests.ToList();
        foreach (var quest in questsToComplete)
        {
            CompleteQuest(quest.Id);
        }
    }
}