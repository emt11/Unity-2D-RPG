using TMPro;
using UnityEngine;

public class UI_Invectory : MonoBehaviour
{
    private Inventory_Player inventory;

    [SerializeField] private UI_ItemSlotParent inventorySlotsParent;
    [SerializeField] private UI_EquipSlotParent uiEquipSlotParent;
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        inventory = FindAnyObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }

    private void OnEnable()
    {
        if (inventory == null)
            return; 
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        inventorySlotsParent.UpdateSlots(inventory.itemList);
        uiEquipSlotParent.UpdateEquipmentSlots(inventory.equipList);
        goldText.text = inventory.gold.ToString("N0") + "g.";
    }
}
