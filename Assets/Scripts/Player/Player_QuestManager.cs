using System;
using System.Collections.Generic;
using UnityEngine;

public class Player_QuestManager : MonoBehaviour, ISaveable
{
    public event Action OnQuestChanged;
    private Inventory_Player inventory;

    [Header("Quest DATABASE")]
    [SerializeField] private QuestDatabaseSO questDatabase;
    
    public List<QuestData> activeQuests;
    public List<QuestData> completedQuests;
    private Entity_DropManager dropManager;

    private void Awake()
    {
        dropManager = GetComponent<Entity_DropManager>();
        inventory = GetComponent<Inventory_Player>();
    }

    public void GetQuestRewardFrom(RewardType npcType)
    {
        foreach (var quest in new List<QuestData>(activeQuests))
        {
            if (!CanClaimReward(quest, npcType))
                continue;

            QuestDataSO data = quest.questSO;

            if (data.questType == QuestType.Delivery)
            {
                inventory.RemoveItemAmount(data.itemToDelivery, data.requiredAmount);
                quest.currentAmount = data.requiredAmount;
            }

            GiveQuestReward(data);
            CompleteQuest(quest);
        }
    }

    public void GiveQuestReward(QuestDataSO questDataSO)
    {
        foreach (var rewardItem in questDataSO.rewardItems)
        {
            if (rewardItem == null || rewardItem.itemData == null) continue;

            for (int i = 0; i < rewardItem.stackSize; ++i)
                dropManager.CreateDropItem(rewardItem.itemData);
        }
    }

    private bool CanClaimReward(QuestData quest, RewardType npcType)
    {
        QuestDataSO data = quest.questSO;

        // 先检查 NPC，避免交给错误的人
        if (data.rewardType != npcType)
            return false;

        if (data.questType == QuestType.Delivery)
        {
            return data.itemToDelivery != null
                && data.requiredAmount > 0
                && inventory.HasItemAmount(
                    data.itemToDelivery, data.requiredAmount);
        }

        return quest.CanGetReward();
    }

    public bool HasClaimableQuest(RewardType npcType)
    {
        return activeQuests.Exists(quest => CanClaimReward(quest, npcType));
    }

    public void AddProcess(string questTargetId, int amount)
    {
        List<QuestData> completedQuest = new List<QuestData>();
        foreach (var quest in activeQuests)
        {
            if (quest.questSO.targetId != questTargetId) continue;
            quest.AddQuestProcess(amount);
            
            if (quest.CanGetReward() && quest.questSO.rewardType == RewardType.None)   
                completedQuest.Add(quest);
        }
        
        foreach (var quest in completedQuest)
        {
            GiveQuestReward(quest.questSO);
            CompleteQuest(quest);
        }
    }

    public void AcceptQuest(QuestDataSO questSO)
    {
        activeQuests.Add(new QuestData(questSO));
        OnQuestChanged?.Invoke();
    }

    public void CompleteQuest(QuestData quest)
    {
        completedQuests.Add(quest);
        activeQuests.Remove(quest);
        OnQuestChanged?.Invoke();
    }

    public bool QuestIsActive(QuestDataSO questToCheck)
    {
        if (questToCheck == null) return false;

        return activeQuests.Find(q => q.questSO == questToCheck) != null;
    }

    public bool QuestIsCompleted(QuestDataSO quest)
    {
        return completedQuests.Exists(q => q.questSO == quest);
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.activeQuests.Clear();
        gameData.completedQuests.Clear();

        foreach (var quest in activeQuests)
        {
            gameData.activeQuests.Add(quest.questSO.saveId, quest.currentAmount);
        }

        foreach (var quest in completedQuests)
        {
            gameData.completedQuests.Add(quest.questSO.saveId, true);
        }
    }

    public void LoadData(GameData gameData)
    {
        activeQuests.Clear();
        completedQuests.Clear();

        foreach (var entry in gameData.activeQuests)
        {
            string questSaveId = entry.Key;
            int process = entry.Value;

            QuestDataSO questDataSO = questDatabase.GetQuestById(questSaveId);
            if (questDataSO == null) continue;

            QuestData questToLoad = new QuestData(questDataSO);
            questToLoad.currentAmount = process;
            activeQuests.Add(questToLoad);
        }

        foreach (var entry in gameData.completedQuests)
        {
            if (!entry.Value) continue;

            QuestDataSO questSO = questDatabase.GetQuestById(entry.Key);
            if (questSO != null)
                completedQuests.Add(new QuestData(questSO));
        }

        OnQuestChanged?.Invoke();
    }

    public int GetQuestProcess(QuestData questData)
    {
        QuestData quest = activeQuests.Find(q => q == questData);
        return quest == null ? 0 : quest.currentAmount;
    }
}
