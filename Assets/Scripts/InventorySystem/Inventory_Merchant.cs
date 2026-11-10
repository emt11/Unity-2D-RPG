using System.Collections.Generic;
using UnityEngine;

public class Inventory_Merchant : Inventory_Base
{
    private Inventory_Player inventory;

    [SerializeField] private ItemListDataSO shopData;
    [SerializeField] private int minItemsAmount = 4;

    protected override void Awake()
    {
        base.Awake();
        FillShopList();
    }

    public void TryBuyItem(Inventory_Item itemToBuy, bool buyFullStack)
    {
        int amountToBuy = buyFullStack ? itemToBuy.stackSize : 1;
        for (int i = 0; i < amountToBuy; ++i)
        {
            if (inventory.gold < itemToBuy.buyPrice)
            {
                Debug.Log("Not enough money!");
                return;
            }

            if (itemToBuy.itemData.itemType == ItemType.Material)
            {
                inventory.storage.AddMaterialToStash(new Inventory_Item(itemToBuy.itemData));
            }
            else
            {
                if (!inventory.CanAddItem(itemToBuy)) return;

                inventory.AddItem(new Inventory_Item(itemToBuy.itemData));
            }
            
            inventory.gold -= itemToBuy.buyPrice;
            RemoveItem(itemToBuy);
        }

        TriggerUpdateUI();
    }
    
    public void TrySellItem(Inventory_Item itemToSell, bool sellFullStack)
    {
        int amountToSell = sellFullStack ? itemToSell.stackSize : 1;
        for (int i = 0; i < amountToSell; ++i)
        {
            inventory.gold += Mathf.FloorToInt(itemToSell.sellPrice);
            inventory.RemoveItem(itemToSell);
        }

        TriggerUpdateUI();
    }

    public void FillShopList()
    {
        itemList.Clear();
        List<Inventory_Item> possibleItems = new List<Inventory_Item>();

        foreach (var itemData in shopData.itemList)
        {
            int randomStack = Random.Range(itemData.minStackSizeAtShop, itemData.maxStackSizeAtShop + 1);
            int finalStack = Mathf.Clamp(randomStack, 1, itemData.maxStackSize);

            Inventory_Item itemToAdd = new Inventory_Item(itemData);
            itemToAdd.stackSize = finalStack;

            possibleItems.Add(itemToAdd);
        }

        int randomItemAmountInShop = Random.Range(minItemsAmount, maxInventorySize + 1);
        int finalItemAmountInShop = Mathf.Clamp(randomItemAmountInShop, 1, possibleItems.Count);
        for (int i = 0; i < finalItemAmountInShop; ++i)
        {
            int randomIndex = Random.Range(0, possibleItems.Count);
            var item = possibleItems[randomIndex];

            if (CanAddItem(item))
            {
                possibleItems.Remove(item);
                AddItem(item);
            }
        }

        TriggerUpdateUI();
    }

    public void SetInventory(Inventory_Player inventory) => this.inventory = inventory;
}
