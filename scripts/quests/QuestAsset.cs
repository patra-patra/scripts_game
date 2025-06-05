using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest")]
public class QuestAsset : ScriptableObject
{
    [Header("Basic Info")]
    public string questId;
    public string title;
    [TextArea(3, 5)]
    public string description;

    [Header("Dependencies")]
    public List<string> prerequisites = new List<string>();

    [Header("Objectives")]
    public List<ObjectiveAsset> objectives = new List<ObjectiveAsset>();

    [Header("Rewards")]
    public RewardAsset reward;

    [Header("Settings")]
    public bool isMainQuest = false;
    public bool canBeAbandoned = true;
    public float timeLimit = 0f; // 0 = no time limit

    public Quest ToQuest()
    {
        var quest = new Quest
        {
            Id = questId,
            Title = title,
            Description = description,
            Prerequisites = new List<string>(prerequisites),
            Objectives = new List<QuestObjective>(),
            Reward = reward?.ToQuestReward(),
            Status = prerequisites.Count == 0 ? QuestStatus.Available : QuestStatus.Locked
        };

        foreach (var objectiveAsset in objectives)
        {
            quest.Objectives.Add(objectiveAsset.ToQuestObjective());
        }

        return quest;
    }

    private void OnValidate()
    {
        // Проверка на пустой ID
        if (string.IsNullOrEmpty(questId))
        {
            questId = name.Replace(" ", "_").ToLower();
        }

        // Проверка на дублирование целей
        var objectiveIds = new HashSet<string>();
        foreach (var objective in objectives)
        {
            if (objective != null && !string.IsNullOrEmpty(objective.id))
            {
                if (objectiveIds.Contains(objective.id))
                {
                    Debug.LogWarning($"Duplicate objective ID found in quest {questId}: {objective.id}");
                }
                objectiveIds.Add(objective.id);
            }
        }
    }
}