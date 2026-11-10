using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private float timeEchoDuration;

    [Header("Attack Upgrade")]
    [SerializeField] private int maxAttacks = 3;
    [SerializeField] private float duplicateChance = 0.3f;

    [Header("Wisp Upgrade")]
    [SerializeField] private float damagePercentHealed = 0.3f;
    [SerializeField] private float cooldownReducedInSeconds = 2f;

    public override void TryUseSkills()
    {
        if (!CanUseSkill())
            return;

        CreateTimeEcho();
        SetSkillOnCoolDown();
    }

    public float GetDamagePercentHealed() => ShouldBeWisp() ? damagePercentHealed : 0f;

    public float GetCooldownReducedInSeconds() => Unlocked(SkillUpgradeType.TimeEcho_CooldownWisp) ? cooldownReducedInSeconds : 0f;

    public bool CanRemoveAllNegativeEffects() => Unlocked(SkillUpgradeType.TimeEcho_CleanseWisp);

    public bool ShouldBeWisp()
    {
        return Unlocked(SkillUpgradeType.TimeEcho_HealWisp)
            || Unlocked(SkillUpgradeType.TimeEcho_CleanseWisp)
            || Unlocked(SkillUpgradeType.TimeEcho_CooldownWisp);
    }

    public float GetDuplicateChance()
    {
        if (!Unlocked(SkillUpgradeType.TimeEcho_ChanceToDuplicate))
            return 0;
        
        return duplicateChance;
    }

    public int GetMaxAttacks()
    {
        if (Unlocked(SkillUpgradeType.TimeEcho_SingleAttack) || Unlocked(SkillUpgradeType.TimeEcho_ChanceToDuplicate))
            return 1;
        else if (Unlocked(SkillUpgradeType.TimeEcho_MultiAttack))
            return maxAttacks;
        else 
            return 0;
    }

    public float GetTimeEchoDuration() {
        return timeEchoDuration;
    }

    public void CreateTimeEcho(Vector3? targetPosition = null)
    {
        Vector3 position = targetPosition ?? transform.position;
        GameObject timeEcho = Instantiate(timeEchoPrefab, position, Quaternion.identity);
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this);
    }
}
