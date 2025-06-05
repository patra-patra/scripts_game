using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class QuestRepository : IQuestRepository
{
    private List<Quest> _quests;
    private readonly string _saveFilePath;
    private readonly string _questDataPath = "Quests";

    public QuestRepository()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, "quest_progress.json");
        _quests = new List<Quest>();
        LoadQuestData();
    }

    public List<Quest> GetAllQuests()
    {
        return _quests.ToList();
    }

    public Quest GetQuestById(string questId)
    {
        return _quests.FirstOrDefault(q => q.Id == questId);
    }

    public void SaveQuestProgress(Quest quest)
    {
        var existingQuest = GetQuestById(quest.Id);
        if (existingQuest != null)
        {
            existingQuest.Status = quest.Status;
            existingQuest.Objectives = quest.Objectives;
            existingQuest.CompletedAt = quest.CompletedAt;
        }

        SaveProgressToFile();
    }

    public void SaveAllQuestProgress(List<Quest> quests)
    {
        _quests = quests.ToList();
        SaveProgressToFile();
    }

    public List<Quest> GetAvailableQuests()
    {
        return _quests.Where(q => q.Status == QuestStatus.Available).ToList();
    }

    public List<Quest> GetActiveQuests()
    {
        return _quests.Where(q => q.Status == QuestStatus.Active).ToList();
    }

    public List<Quest> GetCompletedQuests()
    {
        return _quests.Where(q => q.Status == QuestStatus.Completed).ToList();
    }

    public void LoadQuestData()
    {
        LoadQuestsFromAssets();
        LoadProgressFromFile();
    }

    public void ResetAllQuests()
    {
        foreach (var quest in _quests)
        {
            quest.Status = quest.Prerequisites.Count == 0 ? QuestStatus.Available : QuestStatus.Locked;
            quest.CompletedAt = null;

            foreach (var objective in quest.Objectives)
            {
                objective.IsCompleted = false;
                objective.CurrentProgress = 0;
            }
        }

        SaveProgressToFile();
    }

    public bool QuestExists(string questId)
    {
        return _quests.Any(q => q.Id == questId);
    }

    private void LoadQuestsFromAssets()
    {
        var questAssets = Resources.LoadAll<QuestAsset>(_questDataPath);
        _quests = questAssets.Select(asset => asset.ToQuest()).ToList();

        Debug.Log($"Loaded {_quests.Count} quests from assets");
    }

    private void LoadProgressFromFile()
    {
        if (!File.Exists(_saveFilePath)) return;

        try
        {
            string json = File.ReadAllText(_saveFilePath);
            var progressData = JsonUtility.FromJson<QuestProgressContainer>(json);

            ApplyProgressData(progressData);
            Debug.Log("Quest progress loaded successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load quest progress: {e.Message}");
        }
    }

    private void SaveProgressToFile()
    {
        try
        {
            var progressData = CreateProgressData();
            string json = JsonUtility.ToJson(progressData, true);
            File.WriteAllText(_saveFilePath, json);

            Debug.Log("Quest progress saved successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save quest progress: {e.Message}");
        }
    }

    private QuestProgressContainer CreateProgressData()
    {
        var container = new QuestProgressContainer();
        container.QuestProgresses = new List<QuestProgressData>();

        foreach (var quest in _quests)
        {
            var progressData = new QuestProgressData
            {
                QuestId = quest.Id,
                Status = quest.Status,
                CompletedAt = quest.CompletedAt,
                CompletedObjectives = quest.Objectives
                    .Where(o => o.IsCompleted)
                    .Select(o => o.Id)
                    .ToList(),
                ObjectiveProgresses = quest.Objectives
                    .ToDictionary(o => o.Id, o => o.CurrentProgress)
            };

            container.QuestProgresses.Add(progressData);
        }

        return container;
    }

    private void ApplyProgressData(QuestProgressContainer progressData)
    {
        foreach (var questProgress in progressData.QuestProgresses)
        {
            var quest = GetQuestById(questProgress.QuestId);
            if (quest == null) continue;

            quest.Status = questProgress.Status;
            quest.CompletedAt = questProgress.CompletedAt;

            foreach (var objective in quest.Objectives)
            {
                if (questProgress.CompletedObjectives.Contains(objective.Id))
                {
                    objective.IsCompleted = true;
                }

                if (questProgress.ObjectiveProgresses.ContainsKey(objective.Id))
                {
                    objective.CurrentProgress = questProgress.ObjectiveProgresses[objective.Id];
                }
            }
        }
    }
}