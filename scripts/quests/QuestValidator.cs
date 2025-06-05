using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class QuestValidator
{
    public static bool ValidateQuest(Quest quest)
    {
        if (quest == null)
        {
            Debug.LogError("Quest is null");
            return false;
        }

        bool isValid = true;

        // Проверка базовых полей
        if (string.IsNullOrEmpty(quest.Id))
        {
            Debug.LogError($"Quest has empty ID");
            isValid = false;
        }

        if (string.IsNullOrEmpty(quest.Title))
        {
            Debug.LogError($"Quest {quest.Id} has empty title");
            isValid = false;
        }

        // Проверка целей
        if (quest.Objectives == null || quest.Objectives.Count == 0)
        {
            Debug.LogWarning($"Quest {quest.Id} has no objectives");
        }
        else
        {
            foreach (var objective in quest.Objectives)
            {
                if (!ValidateObjective(objective, quest.Id))
                {
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    public static bool ValidateObjective(QuestObjective objective, string questId = "")
    {
        if (objective == null)
        {
            Debug.LogError($"Null objective in quest {questId}");
            return false;
        }

        bool isValid = true;

        if (string.IsNullOrEmpty(objective.Id))
        {
            Debug.LogError($"Objective has empty ID in quest {questId}");
            isValid = false;
        }

        if (string.IsNullOrEmpty(objective.Description))
        {
            Debug.LogError($"Objective {objective.Id} has empty description in quest {questId}");
            isValid = false;
        }

        if (objective.RequiredProgress <= 0)
        {
            Debug.LogError($"Objective {objective.Id} has invalid required progress: {objective.RequiredProgress}");
            isValid = false;
        }

        // Проверка параметров в зависимости от типа
        switch (objective.Type)
        {
            case ObjectiveType.CollectItems:
                if (!objective.Parameters.ContainsKey("itemId") ||
                    string.IsNullOrEmpty(objective.Parameters["itemId"].ToString()))
                {
                    Debug.LogError($"CollectItems objective {objective.Id} missing itemId parameter");
                    isValid = false;
                }
                break;

            case ObjectiveType.KillEnemies:
                if (!objective.Parameters.ContainsKey("enemyType") ||
                    string.IsNullOrEmpty(objective.Parameters["enemyType"].ToString()))
                {
                    Debug.LogError($"KillEnemies objective {objective.Id} missing enemyType parameter");
                    isValid = false;
                }
                break;

            case ObjectiveType.TalkToNPC:
                if (!objective.Parameters.ContainsKey("npcId") ||
                    string.IsNullOrEmpty(objective.Parameters["npcId"].ToString()))
                {
                    Debug.LogError($"TalkToNPC objective {objective.Id} missing npcId parameter");
                    isValid = false;
                }
                break;

            case ObjectiveType.ReachLocation:
                if (!objective.Parameters.ContainsKey("targetPosition"))
                {
                    Debug.LogError($"ReachLocation objective {objective.Id} missing targetPosition parameter");
                    isValid = false;
                }
                break;
        }

        return isValid;
    }

    public static bool ValidateQuestDependencies(List<Quest> allQuests)
    {
        bool isValid = true;
        var questIds = allQuests.Select(q => q.Id).ToHashSet();

        foreach (var quest in allQuests)
        {
            if (quest.Prerequisites != null)
            {
                foreach (var prereqId in quest.Prerequisites)
                {
                    if (!questIds.Contains(prereqId))
                    {
                        Debug.LogError($"Quest {quest.Id} references non-existent prerequisite: {prereqId}");
                        isValid = false;
                    }
                }
            }
        }

        return isValid;
    }

    public static List<string> FindCircularDependencies(List<Quest> allQuests)
    {
        var circularDeps = new List<string>();
        var questDict = allQuests.ToDictionary(q => q.Id);

        foreach (var quest in allQuests)
        {
            var visited = new HashSet<string>();
            var stack = new HashSet<string>();

            if (HasCircularDependency(quest.Id, questDict, visited, stack))
            {
                circularDeps.Add(quest.Id);
            }
        }

        return circularDeps;
    }

    private static bool HasCircularDependency(string questId, Dictionary<string, Quest> questDict,
        HashSet<string> visited, HashSet<string> stack)
    {
        if (!visited.Contains(questId))
        {
            visited.Add(questId);
            stack.Add(questId);

            if (questDict.ContainsKey(questId))
            {
                var quest = questDict[questId];
                if (quest.Prerequisites != null)
                {
                    foreach (var prereq in quest.Prerequisites)
                    {
                        if (!visited.Contains(prereq) &&
                            HasCircularDependency(prereq, questDict, visited, stack))
                            return true;
                        else if (stack.Contains(prereq))
                            return true;
                    }
                }
            }
        }

        stack.Remove(questId);
        return false;
    }
}