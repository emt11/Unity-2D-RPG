using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ActiveQuestSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questSlotName;
    [SerializeField] private Image[] questSlotIcons;
    private UI_ActiveQuestPreview activeQuestPreview;
    private QuestData questInSlot;

    public void SetupActiveQuestSlot(QuestData questToSetup)
    {
        activeQuestPreview = transform.root.GetComponentInChildren<UI_ActiveQuestPreview>();
        questInSlot = questToSetup;
        
        questSlotName.text = questInSlot.questSO.questName;
        
        foreach (var slot in questSlotIcons)
            slot.gameObject.SetActive(false);
        
        Inventory_Item[] rewardItems = questInSlot.questSO.rewardItems;
        for (int i = 0; i < rewardItems.Length; ++i)
        {
            if (rewardItems[i] == null) return;
            Image previewSlot = questSlotIcons[i];
            previewSlot.gameObject.SetActive(true);
            previewSlot.sprite = rewardItems[i].itemData.itemIcon;
            previewSlot.GetComponentInChildren<TextMeshProUGUI>().text = rewardItems[i].stackSize.ToString();
        }
    }

    public void UpdateActiveQuestPreview()
    {
        activeQuestPreview.SetupActiveQuestPreview(questInSlot);
    }
}
