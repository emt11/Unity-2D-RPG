using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Player player;
    private Inventory_Player inventory;
    private UI_SkillSlot[] skillSlots;
    
    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Quick Item Slots")]
    [SerializeField] private float yOffsetQuickItemParent = 150;
    [SerializeField] private Transform quickItemOptionsParent;
    private UI_QuickItemSlotOption[] quickItemSlotOptions;
    private UI_QuickItemSlot[] quickItemSlots;

    private void Start()
    {
        quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>();
        player = FindAnyObjectByType<Player>();
        player.health.OnHealthUpdate += UpdateHealthBar;

        inventory = player.inventory;
        inventory.OnInventoryChange += UdpateQuickSlotsUI;
        inventory.OnQuickSlotUsed += PlayerQuickSlotFeedback;
    }

    public void PlayerQuickSlotFeedback(int slotNumber) => quickItemSlots[slotNumber].SimulateButtonFeedback();

    public void UdpateQuickSlotsUI()
    {
        Inventory_Item[] quickItems = inventory.quickItems;
        for (int i = 0; i < quickItems.Length; ++i)
            quickItemSlots[i].UpdateQuickSlotUI(quickItems[i]);
    }

    public void OpenQuickItemOptions(UI_QuickItemSlot quickItemSlot, RectTransform rect)
    {
        if (quickItemSlotOptions == null)
            quickItemSlotOptions = GetComponentsInChildren<UI_QuickItemSlotOption>(true);

        List<Inventory_Item> consumables = inventory.itemList.FindAll(item => item.itemData.itemType == ItemType.Consumable);
        for (int i = 0; i < quickItemSlotOptions.Length; ++i)
        {
            if (i < consumables.Count)
            {
                quickItemSlotOptions[i].gameObject.SetActive(true);
                quickItemSlotOptions[i].SetupOption(quickItemSlot, consumables[i]);
            }
            else
                quickItemSlotOptions[i].gameObject.SetActive(false);
        }

        quickItemOptionsParent.position = rect.position + Vector3.up * yOffsetQuickItemParent;
    }

    public void HideQuickItemOptions() => quickItemOptionsParent.position += new Vector3(0, 9999f);

    public UI_SkillSlot GetSkillSlot(SkillType skillType)
    {
        if (skillSlots == null)
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);

        foreach (var slot in skillSlots)
            if (slot.skillType == skillType)
                return slot;

        return null;
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHealth());
        float maxHealth = player.stats.GetMaxHealth();
        float sizeDiff = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);
        if (sizeDiff > 0.1f)
            healthRect.sizeDelta = new Vector2(maxHealth, healthRect.sizeDelta.y);

        healthText.text = currentHealth + "/" + maxHealth;
        healthSlider.value = player.health.GetHealthPercent();
    }
}
