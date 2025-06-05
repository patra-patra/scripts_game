using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestDependencyResolver
{
    private readonly IQuestRepository _questRepository;

    public QuestDependencyResolver(IQuestRepository questRepository)
    {
        _questRepository = questRepository;
    }

    public bool ArePrerequisitesMet(Quest quest)
    {
        if (quest.Prerequisites == null || quest.Prerequisites.Count == 0)
            return true;

        foreach (var prerequisiteId in quest.Prerequisites)
        {
            var prerequisiteQuest = _questRepository.GetQuestById(prerequisiteId);
            if (prerequisiteQuest == null)
            {
                Debug.LogWarning($"Prerequisite quest {prerequisiteId} not found for quest {quest.Id}");
                return false;
            }

            if (prerequisiteQuest.Status != QuestStatus.Completed)
                return false;
        }

        return true;
    }

    public List<Quest> GetDependentQuests(string questId)
    {
        return _questRepository.GetAllQuests()
            .Where(q => q.Prerequisites != null &&
                       q.Prerequisites.Contains(questId))
            .ToList();
    }

    public List<Quest> GetPrerequisiteQuests(Quest quest)
    {
        if (quest.Prerequisites == null || quest.Prerequisites.Count == 0)
            return new List<Quest>();

        return quest.Prerequisites
            .Select(id => _questRepository.GetQuestById(id))
            .Where(q => q != null)
            .ToList();
    }

    public bool HasCircularDependency(Quest quest)
    {
        var visited = new HashSet<string>();
        var stack = new HashSet<string>();

        return HasCircularDependencyRecursive(quest.Id, visited, stack);
    }

    public bool ValidateAllQuestDependencies()
    {
        var allQuests = _questRepository.GetAllQuests();
        bool isValid = true;

        foreach (var quest in allQuests)
        {
            // Проверка на существование всех предпосылок
            if (quest.Prerequisites != null)
            {
                foreach (var prerequisiteId in quest.Prerequisites)
                {
                    if (!_questRepository.QuestExists(prerequisiteId))
                    {
                        Debug.LogError($"Quest {quest.Id} has invalid prerequisite: {prerequisiteId}");
                        isValid = false;
                    }
                }
            }

            // Проверка на циклические зависимости
            if (HasCircularDependency(quest))
            {
                Debug.LogError($"Circular dependency detected for quest: {quest.Id}");
                isValid = false;
            }
        }

        return isValid;
    }

    public List<Quest> GetQuestChain(string questId)
    {
        var chain = new List<Quest>();
        var visited = new HashSet<string>();

        BuildQuestChain(questId, chain, visited);
        return chain;
    }

    public int GetQuestDepth(string questId)
    {
        var quest = _questRepository.GetQuestById(questId);
        if (quest == null || quest.Prerequisites == null || quest.Prerequisites.Count == 0)
            return 0;

        int maxDepth = 0;
        foreach (var prerequisiteId in quest.Prerequisites)
        {
            int depth = GetQuestDepth(prerequisiteId) + 1;
            maxDepth = Mathf.Max(maxDepth, depth);
        }

        return maxDepth;
    }

    private bool HasCircularDependencyRecursive(string questId,
        HashSet<string> visited, HashSet<string> stack)
    {
        if (!visited.Contains(questId))
        {
            visited.Add(questId);
            stack.Add(questId);

            var quest = _questRepository.GetQuestById(questId);
            if (quest?.Prerequisites != null)
            {
                foreach (var prerequisite in quest.Prerequisites)
                {
                    if (!visited.Contains(prerequisite) &&
                        HasCircularDependencyRecursive(prerequisite, visited, stack))
                        return true;
                    else if (stack.Contains(prerequisite))
                        return true;
                }
            }
        }

        stack.Remove(questId);
        return false;
    }

    private void BuildQuestChain(string questId, List<Quest> chain, HashSet<string> visited)
    {
        if (visited.Contains(questId)) return;

        var quest = _questRepository.GetQuestById(questId);
        if (quest == null) return;

        visited.Add(questId);

        // Сначала добавляем предпосылки
        if (quest.Prerequisites != null)
        {
            foreach (var prerequisiteId in quest.Prerequisites)
            {
                BuildQuestChain(prerequisiteId, chain, visited);
            }
        }

        // Затем добавляем текущий квест
        if (!chain.Contains(quest))
        {
            chain.Add(quest);
        }
    }
}