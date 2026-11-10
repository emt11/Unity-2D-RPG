using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;

    public List<Inventory_Item> itemList;
    public SerializableDictionary<string, int> inventory; // {itemSaveId, stackSize}
    public SerializableDictionary<string, int> storageItems; // 同上
    public SerializableDictionary<string, int> storageMaterials; // 同上
    public SerializableDictionary<string, ItemType> equipedItems; // {slotType, itemSaveId}
    
    public int skillPoints;
    public SerializableDictionary<string, bool> skillTreeUI; // {skillName, isUnlock}
    public SerializableDictionary<SkillType, SkillUpgradeType> skillUpgrades;

    public SerializableDictionary<string, bool> unlockedCheckpoints; // {checkpointId, isunlock}
    
    public SerializableDictionary<string, int> activeQuests; // {questSaveId, currentProcess}
    public SerializableDictionary<string, bool> completedQuests; // {questSaveId, isCompleted}

    public string lastScenePlayed;
    public Vector3 lastPlayerPosition;

    public GameData()
    {
        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();
        equipedItems = new SerializableDictionary<string, ItemType>();
        
        skillTreeUI = new SerializableDictionary<string, bool>();
        skillUpgrades = new SerializableDictionary<SkillType, SkillUpgradeType>();
        
        unlockedCheckpoints = new SerializableDictionary<string, bool>();

        activeQuests = new SerializableDictionary<string, int>();
        completedQuests = new SerializableDictionary<string, bool>();
    }
}
