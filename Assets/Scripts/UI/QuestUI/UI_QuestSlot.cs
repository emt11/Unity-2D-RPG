using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] questQuickPreviewSlots;
    private UI_QuestPreview questPreview;

    public QuestDataSO questInSlot { get; private set; }

    public void SetupQuestSlot(QuestDataSO questDataSO)
    {
        questPreview = transform.root.GetComponentInChildren<UI_Quest>().GetQuestPreivew();
        questInSlot = questDataSO;
        questName.text = questDataSO.questName;

        foreach (var previewIcon in questQuickPreviewSlots)
            previewIcon.gameObject.SetActive(false);

        for (int i = 0; i < questDataSO.rewardItems.Length; ++i)
        {
            Inventory_Item rewardItem = questDataSO.rewardItems[i];
            if (rewardItem == null || rewardItem.itemData == null) continue;

            Image slot = questQuickPreviewSlots[i];
            slot.gameObject.SetActive(true);
            slot.sprite = rewardItem.itemData.itemIcon;
            slot.GetComponentInChildren<TextMeshProUGUI>().text = rewardItem.stackSize.ToString();
        }
    }

    public void UpdateQuestPreview()
    {
        questPreview.SetupQuestPreview(questInSlot);
    }
}
