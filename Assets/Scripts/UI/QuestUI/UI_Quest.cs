using UnityEngine;

public class UI_Quest : MonoBehaviour
{
    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private UI_QuestPreview questPreview;
    private UI_QuestSlot[] questSlots;
    public Player_QuestManager questManager { get; private set; }

    private void Awake()
    {
        questSlots = GetComponentsInChildren<UI_QuestSlot>(true);
        questManager = Player.instance.questManager;
    }

    public void SetupQuestUI(QuestDataSO[] questsToSetup)
    {
        foreach (var slot in questSlots)
            slot.gameObject.SetActive(false);

        for (int i = 0; i < questsToSetup.Length; ++i)
        {
            questSlots[i].gameObject.SetActive(true);
            questSlots[i].SetupQuestSlot(questsToSetup[i]);
        }

        questPreview.MakeQuestPreviewEmpty();
        inventorySlots.UpdateSlots(Player.instance.inventory.itemList);
        UpdateQuestList();
    }

    public void UpdateQuestList()
    {
        foreach (UI_QuestSlot questSlot in questSlots)
        {
            if (questSlot.gameObject.activeSelf && !CanTakeQuest(questSlot.questInSlot))
                questSlot.gameObject.SetActive(false);
        }
    }

    private bool CanTakeQuest(QuestDataSO questToCheck)
    {
        return questToCheck != null
            && !questManager.QuestIsActive(questToCheck)
            && !questManager.QuestIsCompleted(questToCheck);
    }

    public UI_QuestPreview GetQuestPreivew() => questPreview;

    private void OnEnable()
    {
        questManager.OnQuestChanged += UpdateQuestList;
        UpdateQuestList();
    }

    private void OnDisable()
    {
        questManager.OnQuestChanged -= UpdateQuestList;
    }
}
