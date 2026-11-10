using System;
using TMPro;
using UnityEngine;

public class UI_Merchant : MonoBehaviour
{
    private Inventory_Player inventory;
    private Inventory_Merchant merchant;

    [SerializeField] private TextMeshProUGUI goldText;
    [Space]
    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private UI_ItemSlotParent merchantSlots;
    [SerializeField] private UI_EquipSlotParent equipSlotParent;

    public void SetupMerchantUI(Inventory_Player inventory, Inventory_Merchant merchant)
    {
        this.inventory = inventory;
        this.merchant = merchant;

        this.inventory.OnInventoryChange += UpdateSlotUI;
        this.merchant.OnInventoryChange += UpdateSlotUI;
        UpdateSlotUI();

        UI_MerchantSlot[] merchantSlots = GetComponentsInChildren<UI_MerchantSlot>();
        foreach (var slot in merchantSlots)
            slot.SetupMerchantUI(merchant);
    }

    private void UpdateSlotUI()
    {
        if (inventory == null) return;
        
        inventorySlots.UpdateSlots(inventory.itemList);
        merchantSlots.UpdateSlots(merchant.itemList);
        equipSlotParent.UpdateEquipmentSlots(inventory.equipList);
        goldText.text = inventory.gold.ToString("N0") + "g.";
    }
}
