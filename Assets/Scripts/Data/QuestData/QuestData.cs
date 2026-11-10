using UnityEngine;

[System.Serializable]
public class QuestData
{
    public QuestDataSO questSO;
    public int currentAmount;
    public bool canGetReward;

    public void AddQuestProcess(int amount = 1)
    {
        currentAmount += amount;
    }

    public bool CanGetReward() => canGetReward = currentAmount >= questSO.requiredAmount;

    public QuestData(QuestDataSO questSO)
    {
        this.questSO = questSO;
    }
}
