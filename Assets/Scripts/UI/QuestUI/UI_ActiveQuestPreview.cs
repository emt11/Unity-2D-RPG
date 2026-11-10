using TMPro;
using UnityEngine;

public class UI_ActiveQuestPreview : MonoBehaviour
{
    private Player_QuestManager questManager;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI process;
    [SerializeField] private UI_QuestRewardSlot[] questRewardSlots;

    public void SetupActiveQuestPreview(QuestData questData)
    {
        questManager = Player.instance.questManager;
        QuestDataSO questDataSO = questData.questSO;

        questName.text = questDataSO.questName;
        questDescription.text = questDataSO.description;
        process.text = questDataSO.goal + " " + questManager.GetQuestProcess(questData).ToString() + "/" + questDataSO.requiredAmount.ToString();
        
        EnableQuestPreviewSlots(false);

        for (int i = 0; i < questDataSO.rewardItems.Length; ++i)
        {
            Inventory_Item reward = questDataSO.rewardItems[i];
            if (reward == null || reward.itemData == null)
                continue;

            Inventory_Item previewItem = new Inventory_Item(reward.itemData)
            {
                stackSize = reward.stackSize
            };
            questRewardSlots[i].gameObject.SetActive(true);
            questRewardSlots[i].UpdateItemSlot(previewItem);
        }
    }

    public void MakeActiveQuestPreviewEmpty()
    {
        questName.text = "";
        questDescription.text = "";
        process.text = "";

        EnableQuestPreviewSlots(false);
    }

    private void EnableQuestPreviewSlots(bool enable)
    {
        foreach (var slot in questRewardSlots)
            slot.gameObject.SetActive(enable);
    }
}
