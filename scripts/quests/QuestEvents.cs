using System;
using UnityEngine;

public static class QuestEvents
{
    // События для сбора предметов
    public static event Action<string> OnItemCollected;
    public static event Action<string, int> OnItemCollectedWithAmount;

    // События для убийства врагов
    public static event Action<string> OnEnemyKilled;
    public static event Action<string, int> OnEnemiesKilled;

    // События для разговоров с NPC
    public static event Action<string, string> OnNPCTalkedTo;
    public static event Action<string> OnDialogueCompleted;

    // События для достижения локаций
    public static event Action<Vector3> OnLocationReached;
    public static event Action<string> OnAreaEntered;
    public static event Action<string> OnAreaExited;

    // События для взаимодействия с объектами
    public static event Action<string> OnObjectInteracted;
    public static event Action<string, string> OnObjectUsed;

    // Пользовательские события
    public static event Action<string, object> OnCustomEvent;

    // Методы для вызова событий
    public static void TriggerItemCollected(string itemId)
    {
        OnItemCollected?.Invoke(itemId);
        Debug.Log($"[QuestEvents] Item collected: {itemId}");
    }

    public static void TriggerItemCollected(string itemId, int amount)
    {
        OnItemCollectedWithAmount?.Invoke(itemId, amount);
        // Также вызываем основное событие для совместимости
        for (int i = 0; i < amount; i++)
        {
            OnItemCollected?.Invoke(itemId);
        }
        Debug.Log($"[QuestEvents] Items collected: {itemId} x{amount}");
    }

    public static void TriggerEnemyKilled(string enemyType)
    {
        OnEnemyKilled?.Invoke(enemyType);
        Debug.Log($"[QuestEvents] Enemy killed: {enemyType}");
    }

    public static void TriggerEnemiesKilled(string enemyType, int count)
    {
        OnEnemiesKilled?.Invoke(enemyType, count);
        // Также вызываем основное событие для каждого врага
        for (int i = 0; i < count; i++)
        {
            OnEnemyKilled?.Invoke(enemyType);
        }
        Debug.Log($"[QuestEvents] Enemies killed: {enemyType} x{count}");
    }

    public static void TriggerNPCTalk(string npcId, string dialogueId = "")
    {
        OnNPCTalkedTo?.Invoke(npcId, dialogueId);
        Debug.Log($"[QuestEvents] Talked to NPC: {npcId}, Dialogue: {dialogueId}");
    }

    public static void TriggerDialogueCompleted(string dialogueId)
    {
        OnDialogueCompleted?.Invoke(dialogueId);
        Debug.Log($"[QuestEvents] Dialogue completed: {dialogueId}");
    }

    public static void TriggerLocationReached(Vector3 position)
    {
        OnLocationReached?.Invoke(position);
        Debug.Log($"[QuestEvents] Location reached: {position}");
    }

    public static void TriggerAreaEntered(string areaId)
    {
        OnAreaEntered?.Invoke(areaId);
        Debug.Log($"[QuestEvents] Area entered: {areaId}");
    }

    public static void TriggerAreaExited(string areaId)
    {
        OnAreaExited?.Invoke(areaId);
        Debug.Log($"[QuestEvents] Area exited: {areaId}");
    }

    public static void TriggerObjectInteracted(string objectId)
    {
        OnObjectInteracted?.Invoke(objectId);
        Debug.Log($"[QuestEvents] Object interacted: {objectId}");
    }

    public static void TriggerObjectUsed(string objectId, string action)
    {
        OnObjectUsed?.Invoke(objectId, action);
        Debug.Log($"[QuestEvents] Object used: {objectId}, Action: {action}");
    }

    public static void TriggerCustomEvent(string eventId, object data = null)
    {
        OnCustomEvent?.Invoke(eventId, data);
        Debug.Log($"[QuestEvents] Custom event: {eventId}");
    }
}