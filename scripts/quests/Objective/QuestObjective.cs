using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestObjective
{
    public string Id { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public ObjectiveType Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public int CurrentProgress { get; set; }
    public int RequiredProgress { get; set; }

    public QuestObjective()
    {
        Parameters = new Dictionary<string, object>();
        CurrentProgress = 0;
        RequiredProgress = 1;
    }

    public void UpdateProgress(int amount)
    {
        CurrentProgress = Mathf.Min(CurrentProgress + amount, RequiredProgress);
        IsCompleted = CurrentProgress >= RequiredProgress;
    }

    public float GetProgressPercentage()
    {
        return RequiredProgress > 0 ? (float)CurrentProgress / RequiredProgress : 0f;
    }

    public string GetProgressText()
    {
        return $"{CurrentProgress}/{RequiredProgress}";
    }
}

public enum ObjectiveType
{
    KillEnemies,
    CollectItems,
    ReachLocation,
    TalkToNPC,
    InteractWithObject,
    Custom
}