using UnityEditor.U2D.Aseprite;
using UnityEngine;

/// <summary>
/// 技能基础对象
/// </summary>
public class Skill_Base : MonoBehaviour
{
    public Player player { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public DamageScaleData damageScaleData { get; private set; }

    [Header("General Details")]
    [SerializeField] protected SkillType skillType;
    [SerializeField] protected SkillUpgradeType upgradeType;
    [SerializeField] protected float cooldown;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        lastTimeUsed = Time.time - cooldown;
        player = GetComponentInParent<Player>();
        skillManager = GetComponentInParent<Player_SkillManager>();
        damageScaleData = new DamageScaleData();
    }

    public virtual void TryUseSkills()
    {
        
    }

    public void SetSkillUpgrade(Skill_DataSO skillData)
    {
        SkillUpgradeData upgradeData = skillData.upgradeData;
        upgradeType = upgradeData.upgradeType;
        cooldown = upgradeData.cooldown;
        damageScaleData = upgradeData.damageScaleData;
        
        player.ui.inGameUI.GetSkillSlot(skillData.skillType)?.SetupSkillSlot(skillData);
        ResetCooldown();
    }

    public virtual bool CanUseSkill()
    {
        if (upgradeType == SkillUpgradeType.None) 
            return false;
            
        if (OnCooldown())
        {
            Debug.LogWarning("On Cooldown");
            return false;
        }

        return true;
    }

    protected bool Unlocked(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    public SkillType GetSkillType() => skillType;

    public SkillUpgradeType GetUpgraderType() => upgradeType;

    public bool OnCooldown() => Time.time < lastTimeUsed + cooldown;

    public void SetSkillOnCoolDown() {
        lastTimeUsed = Time.time;
        player.ui.inGameUI.GetSkillSlot(skillType).StartCooldown(cooldown);
    }
    
    public void ReductCooldownBy(float cooldownReduction) => lastTimeUsed -= cooldownReduction;

    public void ResetCooldown()
    {
        lastTimeUsed = Time.time - cooldown;
        player.ui.inGameUI.GetSkillSlot(skillType).ResetCooldown();  
    } 
}
