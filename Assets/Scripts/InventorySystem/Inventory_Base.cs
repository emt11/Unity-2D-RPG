using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Base : MonoBehaviour, ISaveable
{
    public event Action OnInventoryChange;
    protected Player player;

    [Header("ITEM DATA BASE")]
    [SerializeField] protected ItemListDataSO itemDataBase;

    public int maxInventorySize = 15;
    public List<Inventory_Item> itemList = new List<Inventory_Item>();
    
    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }

    public void TryUseItem(Inventory_Item itemToUse)
    {
        Inventory_Item consumableItem = itemList.Find(item => item == itemToUse);
        if (consumableItem == null) 
            return;

        ItemEffect_DataSO effect = consumableItem.itemEffect;
        if (effect == null || !effect.CanBeUsed(player))
            return;

        effect.ExecuteEffect();

        if (consumableItem.stackSize > 1) 
            consumableItem.DecreaseStackSize();
        else 
            RemoveItem(consumableItem);
        
        OnInventoryChange?.Invoke();
    }

    public void AddItem(Inventory_Item itemToAdd)
    {
        Inventory_Item itemInInventory = FindStackableItem(itemToAdd);
        if (itemInInventory != null)
            itemInInventory.IncreaseStackSize();
        else
            itemList.Add(itemToAdd);
            
        OnInventoryChange?.Invoke();
    }

    public void RemoveItem(Inventory_Item itemToRemove)
    {
        Inventory_Item itemInInventory = FindItem(itemToRemove);
        if (itemInInventory.stackSize > 1)
            itemInInventory.DecreaseStackSize();
        else
            itemList.Remove(itemInInventory);

        OnInventoryChange?.Invoke();
    }

    public void RemoveFullStack(Inventory_Item itemToRemove)
    {
        for (int i = 0; i < itemToRemove.stackSize; ++i)
        {
            RemoveItem(itemToRemove);
        }        
    }

    public void RemoveItemAmount(ItemDataSO itemToRemove, int amount)
    {
        foreach (var item in itemList)
        {
            if (item.itemData != itemToRemove) continue;
            
            int removeSize = Mathf.Min(amount, item.stackSize);
            for (int i = 0; i < removeSize; ++i)
            {
                RemoveItem(item);
                if (--amount <= 0)
                    return;
            }
        }
    }

    public bool HasItemAmount(ItemDataSO itemToCheck, int amount)
    {
        int total = 0;
        foreach (var item in itemList)
        {
            if (item.itemData == itemToCheck)
            {
                total += item.stackSize;
                if (total >= amount)
                    return true;
            }
        }

        return false;
    }

    public Inventory_Item FindItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item == itemToFind);
    }

    public Inventory_Item FindSameItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item.itemData == itemToFind.itemData);
    }

    public bool CanAddItem(Inventory_Item itemToAdd)
    {
        return itemList.Count < maxInventorySize || FindStackableItem(itemToAdd) != null;
    }

    public Inventory_Item FindStackableItem(Inventory_Item itemToAdd)
    {
        return itemList.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());
    }

    public void TriggerUpdateUI() => OnInventoryChange?.Invoke();

    public virtual void SaveData(ref GameData gamedata)
    {
        
    }

    public virtual void LoadData(GameData gamedata)
    {
        
    }
}
