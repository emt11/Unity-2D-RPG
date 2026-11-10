using UnityEngine;
using System;

[CreateAssetMenu(menuName = "RPG Setup/Skill Setup", fileName = "Skill Data - ")]
public class Skill_DataSO : ScriptableObject
{
    [Header("Skill description")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;
    public bool unlockedByDefault;

    [Header("Unlock & Upgrade")]
    public int cost;
    public SkillType skillType;
    public SkillUpgradeData upgradeData;
}

[Serializable]
public class SkillUpgradeData
{
    public SkillUpgradeType upgradeType;
    public float cooldown;
    public DamageScaleData damageScaleData;
}
