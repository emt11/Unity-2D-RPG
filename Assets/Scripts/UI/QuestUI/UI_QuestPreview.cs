using TMPro;
using UnityEngine;

public class UI_QuestPreview : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questGoal;
    [SerializeField] private UI_QuestRewardSlot[] questRewardSlots;
    [SerializeField] private GameObject[] additionalObjects;

    private UI_Quest questUI;
    private QuestDataSO previewQuest;

    public void SetupQuestPreview(QuestDataSO questDataSO)
    {
        questUI = transform.root.GetComponentInChildren<UI_Quest>();
        previewQuest = questDataSO;
        
        questName.text = questDataSO.questName;
        questDescription.text = questDataSO.description;
        questGoal.text = questDataSO.goal + " " + questDataSO.requiredAmount.ToString();

        EnableQuestPreviewSlots(false);
        EnableAdditionalObjects(true);

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

    public void AcceptQuest()
    {
        MakeQuestPreviewEmpty();
        questUI.questManager.AcceptQuest(previewQuest);
    }

    public void MakeQuestPreviewEmpty()
    {
        questName.text = "";
        questDescription.text = "";
        questGoal.text = "";

        EnableQuestPreviewSlots(false);
        EnableAdditionalObjects(false);
    }

    private void EnableQuestPreviewSlots(bool enable)
    {
        foreach (var slot in questRewardSlots)
            slot.gameObject.SetActive(enable);
    }

    private void EnableAdditionalObjects(bool enable)
    {
        foreach (var obj in additionalObjects)
            obj.gameObject.SetActive(enable);
    }
}
