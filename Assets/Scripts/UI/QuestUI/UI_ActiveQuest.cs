using System.Collections.Generic;
using UnityEngine;

public class UI_ActiveQuest : MonoBehaviour
{
    private Player_QuestManager questManager;
    private UI_ActiveQuestSlot[] activeQuestSlots;
    [SerializeField] private UI_ActiveQuestPreview activeQuestPreview;

    private void Awake()
    {
        questManager = Player.instance.questManager;
        activeQuestSlots = GetComponentsInChildren<UI_ActiveQuestSlot>(true);
    }

    private void OnEnable()
    {
        foreach (var quest in activeQuestSlots)
            quest.gameObject.SetActive(false);

        List<QuestData> activeQuests = questManager.activeQuests;
        for (int i = 0; i < activeQuests.Count; ++i)
        {
            var questSlot = activeQuestSlots[i];
            var qusetData = activeQuests[i];
            questSlot.gameObject.SetActive(true);
            questSlot.SetupActiveQuestSlot(qusetData);
        }

        if (activeQuests.Count > 0)
            activeQuestSlots[0].UpdateActiveQuestPreview();
        else
            activeQuestPreview.MakeActiveQuestPreviewEmpty();
    }
}
