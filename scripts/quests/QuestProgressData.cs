using System;
using System.Collections.Generic;

[Serializable]
public class QuestProgressContainer
{
    public List<QuestProgressData> QuestProgresses;
}

[Serializable]
public class QuestProgressData
{
    public string QuestId;
    public QuestStatus Status;
    public DateTime? CompletedAt;
    public List<string> CompletedObjectives;
    public Dictionary<string, int> ObjectiveProgresses;

    public QuestProgressData()
    {
        CompletedObjectives = new List<string>();
        ObjectiveProgresses = new Dictionary<string, int>();
    }
}