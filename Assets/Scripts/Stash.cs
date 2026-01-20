using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stash
{
    [SerializeField] private List<CollectedItem> items = new List<CollectedItem>();

    public event Action<List<CollectedItem>, CollectedItem> OnStashUpdated;
    public event Action OnStashCleared;


    public void AddItem(ItemData itemData, int amount)
    {
        var existingItem = items.FirstOrDefault(i => i.Data == itemData);

        if (existingItem != null)
        {
            existingItem.Amount += amount;

            OnStashUpdated?.Invoke(items, existingItem);
        }
        else
        {
            var newItem = new CollectedItem(itemData, amount);
            items.Add(newItem);

            OnStashUpdated?.Invoke(items, newItem);
        }
    }

    public void Clear()
    {
        items.Clear();
        OnStashCleared?.Invoke();
    }

    public List<CollectedItem> GetItems()
    {
        return items;
    }

    public bool IsEmpty()
    {
        return items.Count == 0;
    }
}
