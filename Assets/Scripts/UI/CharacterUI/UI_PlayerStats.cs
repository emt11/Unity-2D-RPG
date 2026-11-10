using UnityEngine;

public class UI_PlayerStats : MonoBehaviour
{
    private UI_StatSlot[] uiStatSlots;
    private Inventory_Player inventory;

    private void Awake()
    {
        uiStatSlots = GetComponentsInChildren<UI_StatSlot>();
        inventory = FindAnyObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateStatSlotsUI;
    }

    private void Start()
    {
        UpdateStatSlotsUI();
    }

    private void UpdateStatSlotsUI()
    {
        foreach (var statSlot in uiStatSlots)
        {
            statSlot.UpdateStatValue();
        }
    }
}
