using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MerchantSlot : UI_ItemSlot
{
    private Inventory_Merchant merchant;
    public enum MerchantSlotType { MerchantType, PlayerInventorySlot }
    public MerchantSlotType slotType;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        bool leftButton = eventData.button == PointerEventData.InputButton.Left;
        bool rightButton = eventData.button == PointerEventData.InputButton.Right;

        if (slotType == MerchantSlotType.PlayerInventorySlot)
        {
            if (rightButton)
            {
                // sell items
                bool sellFullStack = Input.GetKey(KeyCode.LeftControl);
                merchant.TrySellItem(itemInSlot, sellFullStack);
            }
            else if (leftButton)
            {
                // equip/unequip
                base.OnPointerDown(eventData);
            }
        }
        else if (slotType == MerchantSlotType.MerchantType)
        {
            if (leftButton)
            {
                // buy item to inventory
                bool buyFullStack = Input.GetKey(KeyCode.LeftControl);
                merchant.TryBuyItem(itemInSlot, buyFullStack);
            }
        }

        ui.itemToolTip.ShowToolTip(false, null);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        if (slotType == MerchantSlotType.MerchantType)
            ui.itemToolTip.ShowToolTip(true, rect, itemInSlot, true, true);
        else 
            ui.itemToolTip.ShowToolTip(true, rect, itemInSlot, false, true);
    }

    public void SetupMerchantUI(Inventory_Merchant merchant) => this.merchant = merchant;
}
