using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public QuestStatus Status { get; set; }
    public List<string> Prerequisites { get; set; }
    public List<QuestObjective> Objectives { get; set; }
    public QuestReward Reward { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; } 

    public Quest()
    {
        Prerequisites = new List<string>();
        Objectives = new List<QuestObjective>();
        Status = QuestStatus.Locked;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsCompleted => Status == QuestStatus.Completed;
    public bool IsActive => Status == QuestStatus.Active;
    public bool IsAvailable => Status == QuestStatus.Available;

    public float GetProgress()
    {
        if (Objectives == null || Objectives.Count == 0) return 0f;

        int completedCount = 0;
        foreach (var objective in Objectives)
        {
            if (objective.IsCompleted) completedCount++;
        }

        return (float)completedCount / Objectives.Count;
    }
}

public enum QuestStatus
{
    Locked,
    Available,
    Active,
    Completed,
    Failed
}