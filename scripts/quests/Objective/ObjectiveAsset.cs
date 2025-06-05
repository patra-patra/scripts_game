using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "Quest System/Objective")]
public class ObjectiveAsset : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    [TextArea(2, 3)]
    public string description;
    public ObjectiveType type;

    [Header("Progress")]
    public int requiredProgress = 1;

    [Header("Type-Specific Parameters")]
    public ObjectiveParameters parameters;

    public QuestObjective ToQuestObjective()
    {
        var objective = new QuestObjective
        {
            Id = id,
            Description = description,
            Type = type,
            RequiredProgress = requiredProgress,
            Parameters = parameters.ToDictionary()
        };

        return objective;
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = name.Replace(" ", "_").ToLower();
        }

        if (requiredProgress < 1)
        {
            requiredProgress = 1;
        }
    }
}

[System.Serializable]
public class ObjectiveParameters
{
    [Header("Item Collection")]
    public string itemId;

    [Header("Enemy Killing")]
    public string enemyType;

    [Header("NPC Interaction")]
    public string npcId;
    public string specificDialogueId;

    [Header("Location")]
    public Vector3 targetPosition;
    public float radius = 5f;
    public string areaId;

    [Header("Object Interaction")]
    public string objectId;
    public string requiredAction;

    [Header("Custom")]
    public List<CustomParameter> customParameters = new List<CustomParameter>();

    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(itemId))
            dict["itemId"] = itemId;

        if (!string.IsNullOrEmpty(enemyType))
            dict["enemyType"] = enemyType;

        if (!string.IsNullOrEmpty(npcId))
            dict["npcId"] = npcId;

        if (!string.IsNullOrEmpty(specificDialogueId))
            dict["dialogueId"] = specificDialogueId;

        if (targetPosition != Vector3.zero)
        {
            dict["targetPosition"] = targetPosition;
            dict["radius"] = radius;
        }

        if (!string.IsNullOrEmpty(areaId))
            dict["areaId"] = areaId;

        if (!string.IsNullOrEmpty(objectId))
            dict["objectId"] = objectId;

        if (!string.IsNullOrEmpty(requiredAction))
            dict["requiredAction"] = requiredAction;

        foreach (var param in customParameters)
        {
            if (!string.IsNullOrEmpty(param.key))
            {
                dict[param.key] = param.value;
            }
        }

        return dict;
    }
}

[System.Serializable]
public class CustomParameter
{
    public string key;
    public string value;
}