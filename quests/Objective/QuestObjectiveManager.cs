using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestObjectiveManager : MonoBehaviour
{
    public event Action<Quest, QuestObjective> OnObjectiveCompleted;
    public event Action<Quest> OnAllObjectivesCompleted;

    [SerializeField] private bool _debugMode = false;

    public void UpdateObjective(string questId, string objectiveId,
        Dictionary<string, object> progress)
    {
        var quest = QuestManager.Instance.GetActiveQuest(questId);
        if (quest == null) return;

        var objective = quest.Objectives.FirstOrDefault(o => o.Id == objectiveId);
        if (objective == null) return;

        bool wasCompleted = objective.IsCompleted;
        ProcessObjectiveProgress(objective, progress);

        if (!wasCompleted && objective.IsCompleted)
        {
            OnObjectiveCompleted?.Invoke(quest, objective);
            LogDebug($"Objective completed: {objective.Description}");

            // Проверяем, все ли цели выполнены
            if (quest.Objectives.All(o => o.IsCompleted))
            {
                OnAllObjectivesCompleted?.Invoke(quest);
                LogDebug($"All objectives completed for quest: {quest.Title}");
            }
        }
    }

    public void UpdateObjectivesForItemCollection(string itemId)
    {
        var activeQuests = QuestManager.Instance.GetActiveQuests();

        foreach (var quest in activeQuests)
        {
            var collectObjectives = quest.Objectives.Where(o =>
                o.Type == ObjectiveType.CollectItems &&
                !o.IsCompleted &&
                o.Parameters.ContainsKey("itemId") &&
                o.Parameters["itemId"].ToString() == itemId).ToList();

            foreach (var objective in collectObjectives)
            {
                objective.UpdateProgress(1);

                if (objective.IsCompleted)
                {
                    OnObjectiveCompleted?.Invoke(quest, objective);
                    LogDebug($"Item collection objective completed: {objective.Description}");
                }
            }

            // Проверяем завершение всех целей квеста
            if (quest.Objectives.All(o => o.IsCompleted))
            {
                OnAllObjectivesCompleted?.Invoke(quest);
            }
        }
    }

    public void UpdateObjectivesForEnemyKill(string enemyType)
    {
        var activeQuests = QuestManager.Instance.GetActiveQuests();

        foreach (var quest in activeQuests)
        {
            var killObjectives = quest.Objectives.Where(o =>
                o.Type == ObjectiveType.KillEnemies &&
                !o.IsCompleted &&
                o.Parameters.ContainsKey("enemyType") &&
                o.Parameters["enemyType"].ToString() == enemyType).ToList();

            foreach (var objective in killObjectives)
            {
                objective.UpdateProgress(1);

                if (objective.IsCompleted)
                {
                    OnObjectiveCompleted?.Invoke(quest, objective);
                    LogDebug($"Kill objective completed: {objective.Description}");
                }
            }

            if (quest.Objectives.All(o => o.IsCompleted))
            {
                OnAllObjectivesCompleted?.Invoke(quest);
            }
        }
    }

    public void UpdateObjectivesForNPCTalk(string npcId, string dialogueId)
    {
        var activeQuests = QuestManager.Instance.GetActiveQuests();

        foreach (var quest in activeQuests)
        {
            var talkObjectives = quest.Objectives.Where(o =>
                o.Type == ObjectiveType.TalkToNPC &&
                !o.IsCompleted &&
                o.Parameters.ContainsKey("npcId") &&
                o.Parameters["npcId"].ToString() == npcId).ToList();

            foreach (var objective in talkObjectives)
            {
                // Проверяем конкретный диалог, если указан
                if (objective.Parameters.ContainsKey("dialogueId"))
                {
                    if (objective.Parameters["dialogueId"].ToString() != dialogueId)
                        continue;
                }

                objective.UpdateProgress(1);

                if (objective.IsCompleted)
                {
                    OnObjectiveCompleted?.Invoke(quest, objective);
                    LogDebug($"Talk objective completed: {objective.Description}");
                }
            }

            if (quest.Objectives.All(o => o.IsCompleted))
            {
                OnAllObjectivesCompleted?.Invoke(quest);
            }
        }
    }

    public void UpdateObjectivesForLocationReached(Vector3 location)
    {
        var activeQuests = QuestManager.Instance.GetActiveQuests();

        foreach (var quest in activeQuests)
        {
            var locationObjectives = quest.Objectives.Where(o =>
                o.Type == ObjectiveType.ReachLocation &&
                !o.IsCompleted).ToList();

            foreach (var objective in locationObjectives)
            {
                if (objective.Parameters.ContainsKey("targetPosition") &&
                    objective.Parameters.ContainsKey("radius"))
                {
                    var targetPos = (Vector3)objective.Parameters["targetPosition"];
                    var radius = (float)objective.Parameters["radius"];

                    if (Vector3.Distance(location, targetPos) <= radius)
                    {
                        objective.UpdateProgress(1);

                        if (objective.IsCompleted)
                        {
                            OnObjectiveCompleted?.Invoke(quest, objective);
                            LogDebug($"Location objective completed: {objective.Description}");
                        }
                    }
                }
            }

            if (quest.Objectives.All(o => o.IsCompleted))
            {
                OnAllObjectivesCompleted?.Invoke(quest);
            }
        }
    }

    public void UpdateCustomObjective(string questId, string objectiveId,
        int progressAmount = 1)
    {
        var quest = QuestManager.Instance.GetActiveQuest(questId);
        if (quest == null) return;

        var objective = quest.Objectives.FirstOrDefault(o =>
            o.Id == objectiveId && o.Type == ObjectiveType.Custom);

        if (objective == null || objective.IsCompleted) return;

        objective.UpdateProgress(progressAmount);

        if (objective.IsCompleted)
        {
            OnObjectiveCompleted?.Invoke(quest, objective);
            LogDebug($"Custom objective completed: {objective.Description}");

            if (quest.Objectives.All(o => o.IsCompleted))
            {
                OnAllObjectivesCompleted?.Invoke(quest);
            }
        }
    }

    private void ProcessObjectiveProgress(QuestObjective objective,
        Dictionary<string, object> progress)
    {
        switch (objective.Type)
        {
            case ObjectiveType.CollectItems:
                ProcessItemCollectionProgress(objective, progress);
                break;
            case ObjectiveType.KillEnemies:
                ProcessKillProgress(objective, progress);
                break;
            case ObjectiveType.ReachLocation:
                ProcessLocationProgress(objective, progress);
                break;
            case ObjectiveType.TalkToNPC:
                ProcessTalkProgress(objective, progress);
                break;
            case ObjectiveType.InteractWithObject:
                ProcessInteractionProgress(objective, progress);
                break;
            case ObjectiveType.Custom:
                ProcessCustomProgress(objective, progress);
                break;
        }
    }

    private void ProcessItemCollectionProgress(QuestObjective objective,
        Dictionary<string, object> progress)
    {
        if (progress.ContainsKey("amount"))
        {
            int amount = Convert.ToInt32(progress["amount"]);
            objective.UpdateProgress(amount);
        }
    }

    private void ProcessKillProgress(QuestObjective objective,
        Dictionary<string, object> progress)
    {
        if (progress.ContainsKey("count"))
        {
            int count = Convert.ToInt32(progress["count"]);
            objective.UpdateProgress(count);
        }
    }

    private void ProcessLocationProgress(QuestObjective objective,
        Dictionary<string, object> progress)
    {
        if (progress.ContainsKey("reached") && (bool)progress["reached"])
        {
            objective.UpdateProgress(1);
        }
    }

    private void ProcessTalkProgress(QuestObjective objective,
        Dictionary<string, object> progress)
    {
        if (progress.ContainsKey("talked") && (bool)progress["talked"])
        {
            objective.UpdateProgress(1);
        }
    }

    private void ProcessInteractionProgress(QuestObjective objective,
        Dictionary<string, object> progress)
    {
        if (progress.ContainsKey("interacted") && (bool)progress["interacted"])
        {
            objective.UpdateProgress(1);
        }
    }

    private void ProcessCustomProgress(QuestObjective objective,
        Dictionary<string, object> progress)
    {
        if (progress.ContainsKey("progress"))
        {
            int progressAmount = Convert.ToInt32(progress["progress"]);
            objective.UpdateProgress(progressAmount);
        }
    }

    private void LogDebug(string message)
    {
        if (_debugMode)
        {
            Debug.Log($"[QuestObjectiveManager] {message}");
        }
    }
}