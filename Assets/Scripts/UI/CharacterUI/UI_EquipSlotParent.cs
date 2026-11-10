using System.Collections.Generic;
using UnityEngine;

public class UI_EquipSlotParent : MonoBehaviour
{
    private UI_EquipSlot[] equipSlots;

    public void UpdateEquipmentSlots(List<Inventory_EquipmentSlot> equipList)
    {
        if (equipSlots == null)
            equipSlots = GetComponentsInChildren<UI_EquipSlot>();

        for (int i = 0; i < equipSlots.Length; ++i)
        {
            Inventory_EquipmentSlot playerEquipSlot = equipList[i];
            if (playerEquipSlot.HasItem())
                equipSlots[i].UpdateItemSlot(playerEquipSlot.equipedItem);
            else
                equipSlots[i].UpdateItemSlot(null);
        }
    }
}
