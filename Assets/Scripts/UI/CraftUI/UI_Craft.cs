using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    [SerializeField] private UI_ItemSlotParent inventorySlotParent;
    private Inventory_Player inventory;
    private UI_CraftListButton[] craftListButtons;
    private UI_CraftSlot[] craftSlots;
    private UI_CraftPreview craftPreview;

    private void SetupCraftListButtons()
    {
        craftSlots = GetComponentsInChildren<UI_CraftSlot>(true);
        craftListButtons = GetComponentsInChildren<UI_CraftListButton>(true);

        foreach (var slot in craftSlots)
            slot.gameObject.SetActive(false);

        foreach (var button in craftListButtons)
            button.SetCraftSlots(craftSlots);
    }

    public void SetupCraftUI(Inventory_Storage storage)
    {
        inventory = storage.playerInventory;
        inventory.OnInventoryChange += UpdateUI;
        UpdateUI();

        craftPreview = GetComponentInChildren<UI_CraftPreview>(true);
        craftPreview.SetupCraftPreview(storage);
        SetupCraftListButtons();
    }

    private void UpdateUI() => inventorySlotParent.UpdateSlots(inventory.itemList);
}
