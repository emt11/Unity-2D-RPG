using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory { get; private set; }
    public List<Inventory_Item> materialStash; // endless

    public void CraftItem(Inventory_Item itemToCraft)
    {
        ConsumeMaterials(itemToCraft);
        playerInventory.AddItem(itemToCraft);
    }

    public bool CanCraftItem(Inventory_Item itemToCraft)
    {
        return HasEnoughMaterials(itemToCraft) && playerInventory.CanAddItem(itemToCraft);
    }

    public void ConsumeMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredItem in itemToCraft.itemData.craftRecipe)
        {
            int remainingAmount = requiredItem.stackSize;
            remainingAmount = ConsumeMaterialsFromList(materialStash, requiredItem.itemData, remainingAmount);

            if (remainingAmount > 0) 
                remainingAmount = ConsumeMaterialsFromList(itemList, requiredItem.itemData, remainingAmount);

            if (remainingAmount > 0)
                remainingAmount = ConsumeMaterialsFromList(playerInventory.itemList, requiredItem.itemData, remainingAmount);
        }
    }

    private int ConsumeMaterialsFromList(List<Inventory_Item> itemList, ItemDataSO material, int remainingAmount)
    {
        foreach (var item in itemList)
        {
            if (remainingAmount <= 0)
                break;

            if (item.itemData != material)
                continue;

            int consumedAmount = Mathf.Min(remainingAmount, item.stackSize);

            remainingAmount -= consumedAmount;
            item.stackSize -= consumedAmount;
        }

        itemList.RemoveAll(item => item.stackSize == 0);
        return remainingAmount;
    }

    private bool HasEnoughMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredItem in itemToCraft.itemData.craftRecipe)
            if (GetAvaliableAmountOf(requiredItem.itemData) < requiredItem.stackSize)
                return false;

        return true;
    }

    public int GetAvaliableAmountOf(ItemDataSO itemData)
    {
        int amount = 0;

        foreach (var item in playerInventory.itemList)
            if (item.itemData == itemData)
                amount += item.stackSize;

        foreach (var item in itemList)
            if (item.itemData == itemData)
                amount += item.stackSize;

        foreach (var item in materialStash)
            if (item.itemData == itemData)
                amount += item.stackSize;

        return amount;
    }

    public void AddMaterialToStash(Inventory_Item itemToAdd)
    {
        var stackableItem = StackableInStash(itemToAdd);
        if (stackableItem != null) 
            stackableItem.IncreaseStackSize();
        else
            materialStash.Add(new Inventory_Item(itemToAdd.itemData));
        
        TriggerUpdateUI();
        materialStash = materialStash.OrderBy(item => item.itemData.name).ToList();
    }

    private Inventory_Item StackableInStash(Inventory_Item itemToAdd)
    {
        return materialStash.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());
    }
    
    public void SetPlayerInventory(Inventory_Player inventory) => this.playerInventory = inventory;

    public void FromPlayerToStorage(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;
        for (int i = 0; i < transferAmount; ++i)
        {
            if (CanAddItem(item))
            {
                playerInventory.RemoveItem(item);
                var itemToAdd = new Inventory_Item(item.itemData);
                AddItem(itemToAdd);
            }
        }

        TriggerUpdateUI();
    }

    public void FromStorageToPlayer(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;
        for (int i = 0; i < transferAmount; ++i) {
            if (playerInventory.CanAddItem(item))
            {
                RemoveItem(item);
                var itemToAdd = new Inventory_Item(item.itemData);
                playerInventory.AddItem(itemToAdd);
            }
        }

        TriggerUpdateUI();
    }

    public override void SaveData(ref GameData gamedata)
    {
        gamedata.storageItems.Clear();
        gamedata.storageMaterials.Clear();

        foreach (var item in itemList)
        {
            if (item == null || item.itemData == null)
                continue;
            
            string saveId = item.itemData.saveId;
            if (!gamedata.storageItems.ContainsKey(saveId))
                gamedata.storageItems[saveId] = 0;
            gamedata.storageItems[saveId] += item.stackSize;
        }

        foreach (var item in materialStash)
        {
            if (item == null || item.itemData == null)
                continue;
            
            string saveId = item.itemData.saveId;
            if (!gamedata.storageMaterials.ContainsKey(saveId))
                gamedata.storageMaterials[saveId] = 0;
            gamedata.storageMaterials[saveId] += item.stackSize;
        }
    }

    public override void LoadData(GameData gamedata)
    {
        itemList.Clear();
        materialStash.Clear();

        foreach (var entry in gamedata.storageItems)
        {
            string saveId = entry.Key;
            int stackSize = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);
            if (itemData == null)
            {
                Debug.Log("Item Not Found : " + saveId);
                continue;
            }

            for (int i = 0; i < stackSize; ++i)
                AddItem(new Inventory_Item(itemData));
        }

        foreach (var entry in gamedata.storageMaterials)
        {
            string saveId = entry.Key;
            int stackSize = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);
            if (itemData == null)
            {
                Debug.Log("Item Not Found : " + saveId);
                continue;
            }

            for (int i = 0; i < stackSize; ++i)
                AddMaterialToStash(new Inventory_Item(itemData));
        }

        TriggerUpdateUI();
    }
}
