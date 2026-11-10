using UnityEngine;

[System.Serializable]
public class DialogueNpcData
{
    public RewardType rewardType;
    public QuestDataSO[] quests;

    public DialogueNpcData(RewardType rewardType, QuestDataSO[] quests)
    {
        this.rewardType = rewardType;
        this.quests = quests;
    }
}
