using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestReward
{
    public int Experience { get; set; }
    public int Gold { get; set; }
    public List<ItemReward> Items { get; set; }

    public QuestReward()
    {
        Items = new List<ItemReward>();
    }

    public bool HasRewards()
    {
        return Experience > 0 || Gold > 0 || (Items != null && Items.Count > 0);
    }
}

[Serializable]
public class ItemReward
{
    public string ItemId { get; set; }
    public int Quantity { get; set; }

    public ItemReward(string itemId, int quantity)
    {
        ItemId = itemId;
        Quantity = quantity;
    }
}