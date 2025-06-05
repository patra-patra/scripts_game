using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Quest System/Reward")]
public class RewardAsset : ScriptableObject
{
    [Header("Basic Rewards")]
    public int experience;
    public int gold;

    [Header("Item Rewards")]
    public List<ItemRewardData> items = new List<ItemRewardData>();

    public QuestReward ToQuestReward()
    {
        var reward = new QuestReward
        {
            Experience = experience,
            Gold = gold,
            Items = new List<ItemReward>()
        };

        foreach (var item in items)
        {
            reward.Items.Add(new ItemReward(item.itemId, item.quantity));
        }

        return reward;
    }

    private void OnValidate()
    {
        if (experience < 0) experience = 0;
        if (gold < 0) gold = 0;

        foreach (var item in items)
        {
            if (item.quantity < 1) item.quantity = 1;
        }
    }
}

[System.Serializable]
public class ItemRewardData
{
    public string itemId;
    public int quantity = 1;

    [Header("Optional")]
    public Sprite icon;
    public string displayName;
}