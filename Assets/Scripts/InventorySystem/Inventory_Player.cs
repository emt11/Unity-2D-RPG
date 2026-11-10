using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    public event Action<int> OnQuickSlotUsed;

    public Inventory_Storage storage { get; private set; }
    public List<Inventory_EquipmentSlot> equipList;

    [Header("Quick item slots")]
    [SerializeField] public Inventory_Item[] quickItems = new Inventory_Item[2];
    
    [Header("Gold info")]
    public int gold = 10000;

    protected override void Awake()
    {
        base.Awake();
        storage = FindAnyObjectByType<Inventory_Storage>();
    }

    public void SetupQuickItemSlot(int slotNumber, Inventory_Item itemToSet)
    {
        quickItems[slotNumber - 1] = itemToSet;
        TriggerUpdateUI();
    }

    public void TryUseQuickItemSlot(int passedSlotNumber)
    {
        int slotNumber = passedSlotNumber - 1;
        var itemToUse = quickItems[slotNumber];
        if (itemToUse == null) return;
        TryUseItem(itemToUse);

        if (FindItem(itemToUse) == null)
        {
            // 用完了，寻找同类加上
            quickItems[slotNumber] = FindSameItem(itemToUse);
        }
        TriggerUpdateUI();
        OnQuickSlotUsed?.Invoke(slotNumber);
    }

    public void TryEquipItem(Inventory_Item itemInSlot)
    {
        Inventory_Item itemInInventory = FindItem(itemInSlot);
        List<Inventory_EquipmentSlot> matchingSlots = equipList.FindAll(slot => slot.slotType == itemInSlot.itemData.itemType);

        foreach (var slot in matchingSlots)
        {
            if (!slot.HasItem())
            {
                EquipItem(itemInInventory, slot);
                return;
            }
        }

        Inventory_EquipmentSlot equipSlotToReplace = matchingSlots[0]; // 默认替换同类型装备的第 0 个格子
        Inventory_Item itemToUnequip = equipSlotToReplace.equipedItem;
        EquipItem(itemInInventory, equipSlotToReplace);
        UnEquipItem(itemToUnequip);
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        float savedHealthPercent = player.health.GetHealthPercent();

        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifiers(player.stats);
        slot.equipedItem.AddItemEffect(player);

        player.health.SetHealthPercent(savedHealthPercent);

        RemoveItem(itemToEquip);
    }

    public void UnEquipItem(Inventory_Item itemToUnEquip)
    {
        if (!CanAddItem(itemToUnEquip))
        {
            Debug.Log("No Space For UnEquip.");
            return;
        }

        Inventory_EquipmentSlot slotToUnEquip = equipList.Find(slot => slot.equipedItem == itemToUnEquip);
        if (slotToUnEquip != null)
            slotToUnEquip.equipedItem = null;

        itemToUnEquip.RemoveItemEffect();

        float savedHealthPercent = player.health.GetHealthPercent();
        itemToUnEquip.RemoveModifiers(player.stats);
        player.health.SetHealthPercent(savedHealthPercent);

        AddItem(itemToUnEquip);
    }

    public override void SaveData(ref GameData gamedata)
    {
        gamedata.gold = gold;
        gamedata.inventory.Clear();
        gamedata.equipedItems.Clear();

        foreach (var item in itemList)
        {
            if (item == null || item.itemData == null) continue;

            string saveId = item.itemData.saveId;
            if (!gamedata.inventory.ContainsKey(saveId))
                gamedata.inventory[saveId] = 0;
            gamedata.inventory[saveId] += item.stackSize;
        }

        foreach (var equip in equipList)
        {
            if (!equip.HasItem()) continue;

            gamedata.equipedItems[equip.equipedItem.itemData.saveId] = equip.slotType;
        }
    }

    public override void LoadData(GameData gamedata)
    {
        gold = gamedata.gold;
        itemList.Clear();

        foreach (var item in gamedata.inventory)
        {
            string saveId = item.Key;
            int stackSize = item.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);
            if (itemData == null)
            {
                Debug.Log("Item Not Found : " + saveId);
                continue;
            }
            
            for (int i = 0; i < stackSize; ++i)
                AddItem(new Inventory_Item(itemData));
        }

        // 清除当前装备及其属性、效果
        // foreach (var slot in equipList)
        // {
        //     if (slot.HasItem())
        //     {
        //         slot.equipedItem.RemoveModifiers(player.stats);
        //         slot.equipedItem.RemoveItemEffect();
        //     }

        //     slot.equipedItem = null;
        // }

        foreach (var entry in gamedata.equipedItems)
        {
            string saveId = entry.Key;
            ItemType loadedSlotType = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveId);
            Inventory_Item equipToLoad = new Inventory_Item(itemData);

            Inventory_EquipmentSlot slot = equipList.Find(slot => slot.slotType == loadedSlotType && !slot.HasItem());
            
            slot.equipedItem = equipToLoad;
            slot.equipedItem.AddModifiers(player.stats);
            slot.equipedItem.AddItemEffect(player);
        }

        player.health.SetHealthPercent(1f); // 默认满血
        TriggerUpdateUI();
    }
}
