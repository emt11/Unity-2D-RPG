using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    [SerializeField] private UI_ItemSlotParent inventorySlotsParent;
    [SerializeField] private UI_ItemSlotParent storageSlotsParent;
    [SerializeField] private UI_ItemSlotParent materialSlotsParent;

    public void SetupStorageUI(Inventory_Storage storage)
    {
        this.storage = storage;
        inventory = storage.playerInventory;
        storage.OnInventoryChange += UpdateUI;
        UpdateUI();

        UI_StorageSlot[] storageSlots = GetComponentsInChildren<UI_StorageSlot>();
        foreach (var slot in storageSlots)
            slot.SetStorage(storage);
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (storage == null) return;
        
        inventorySlotsParent.UpdateSlots(inventory.itemList);
        storageSlotsParent.UpdateSlots(storage.itemList);
        materialSlotsParent.UpdateSlots(storage.materialStash);
    }
}
