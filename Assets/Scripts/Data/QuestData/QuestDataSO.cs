using UnityEditor;
using UnityEngine;

public enum RewardType { Merchant, Blacksmith, None }

public enum QuestType { Kill, Talk, Delivery }

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/New Quest", fileName = "Quest - ")]
public class QuestDataSO : ScriptableObject
{
    public string saveId;
    [Space]
    public string questName;
    [TextArea] public string description;
    [TextArea] public string goal;

    public string targetId;
    public int requiredAmount;

    [Header("Reward")]
    public RewardType rewardType;
    public Inventory_Item[] rewardItems;

    [Header("Delivery")]
    public QuestType questType;
    public ItemDataSO itemToDelivery;

    public void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        saveId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}
