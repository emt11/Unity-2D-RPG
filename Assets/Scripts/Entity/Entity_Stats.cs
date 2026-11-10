using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup;

    public Stat_ResourceGroup resources;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;
    public Stat_MajorGroup major;

    protected virtual void Awake()
    {
        
    }

    public AttackData GetAttackData(DamageScaleData damageScaleData)
    {
        return new AttackData(this, damageScaleData);
    }

    public float GetPhysicalDamage(out bool isCrit, float scaleFactor = 1)
    {
        float totalDamage = GetBaseDamage();
        float totalCritChance = GetCritChance();
        float totalCritPower = GetCritPower() / 100; // 作为倍数

        isCrit = Random.Range(0, 100) < totalCritChance;
        float finalPhysicalDamage = isCrit ? totalDamage * totalCritPower : totalDamage;
        
        return finalPhysicalDamage;
    }

    // Bonus damage from Strength: +1 per STR
    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue();
    // Bonus crit chance from Agility: +0.3% per AGI 
    public float GetCritChance() => offense.critChance.GetValue() + (major.agility.GetValue() * .3f);
    // Bonus crit chance from Strength: +0.5% per STR 
    public float GetCritPower() => offense.critPower.GetValue() + (major.strength.GetValue() * .5f);

    // 返回倍数
    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * 0.5f;
        
        if (element == ElementType.Fire) 
            baseResistance = defense.fireRes.GetValue();
        else if (element == ElementType.Ice)
            baseResistance = defense.iceRes.GetValue();
        else if (element == ElementType.Lightning)
            baseResistance = defense.lightningRes.GetValue();
        
        float totalResistance = (baseResistance + bonusResistance) / 100;
        float resistanceCap = 0.75f;
        float finalResistance = Mathf.Clamp(totalResistance, 0, resistanceCap);

        return finalResistance;
    }

    public float GetElementalDamage(out ElementType element, float scaleFactor = 1)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();
        float bonusElementalDamage = major.intelligence.GetValue();

        float highestDamage = fireDamage;
        element = ElementType.Fire;
        if (iceDamage > highestDamage)
        {
            highestDamage = iceDamage;
            element = ElementType.Ice;
        }
        if (lightningDamage > highestDamage)
        {
            highestDamage = lightningDamage;
            element = ElementType.Lightning;
        }
        if (highestDamage <= 0)
        {
            element = ElementType.None;
            return 0;
        }

        float bonusFireDamage = element == ElementType.Fire ? 0 : fireDamage * 0.5f;
        float bonusIceDamage = element == ElementType.Ice ? 0 : iceDamage * 0.5f;
        float bonusLightningDamage = element == ElementType.Lightning ? 0 : lightningDamage * 0.5f;
        float weakerElementalDamage = bonusFireDamage + bonusIceDamage + bonusLightningDamage;
        
        float finalElementalDamage = highestDamage + weakerElementalDamage + bonusElementalDamage;

        return finalElementalDamage;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();

        float reductionMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100);
        float mitigationCap = 0.85f;
        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalMitigation;
    }

    public float GetBaseArmor() => defense.armor.GetValue() + major.vitality.GetValue();

    public float GetArmorReduction()
    {
        float finalArmorReduction = offense.armorReduction.GetValue() / 100;

        return finalArmorReduction;
    }

    public float GetMaxHealth()
    {
        float baseHp = resources.maxHealth.GetValue();
        float bonusHp = major.vitality.GetValue() * 5;
        float finalHp = baseHp + bonusHp;

        return finalHp;
    }

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f;
        float totalEvasion = baseEvasion + bonusEvasion;

        float evasionCap = 85f;
        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap); // 设置闪避上限

        return finalEvasion;
    }

    public Stat GetStatByType(StatType type)
    {
        switch (type) {
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;
            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Vitality: return major.vitality;
            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritPower: return offense.critPower;
            case StatType.CritChance: return offense.critChance;
            case StatType.ArmorReduction: return offense.armorReduction;
            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage: return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;
            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;
            case StatType.FireResistance: return defense.fireRes;
            case StatType.IceResistance: return defense.iceRes;
            case StatType.LightningResistance: return defense.lightningRes;
            default:
                // Debug.Log($"StatType {type} not implemented yet.");
                return null;
        }
    }

    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefaultStatSetup()
    {
        if (defaultStatSetup == null)
        {
            Debug.Log("No default stat setup assigned");
            return;
        }
        resources.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);
        offense.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offense.damage.SetBaseValue(defaultStatSetup.damage);
        offense.critChance.SetBaseValue(defaultStatSetup.critChance);
        offense.critPower.SetBaseValue(defaultStatSetup.critPower);
        offense.armorReduction.SetBaseValue(defaultStatSetup.armorReduction);
        offense.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offense.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        offense.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);
        defense.armor.SetBaseValue(defaultStatSetup.armor);
        defense.evasion.SetBaseValue(defaultStatSetup.evasion);
        defense.fireRes.SetBaseValue(defaultStatSetup.fireRes);
        defense.iceRes.SetBaseValue(defaultStatSetup.iceRes);
        defense.lightningRes.SetBaseValue(defaultStatSetup.lightningRes);
        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        major.vitality.SetBaseValue(defaultStatSetup.vitality);
    }

    public void AdjustStatSetup(Entity_Stats stats, float penalty, float increase)
    {
        Stat_OffenseGroup offenseGroup = stats.offense;
        Stat_ResourceGroup resourceGroup = stats.resources;
        Stat_DefenseGroup defenseGroup = stats.defense;

        // INCREASE STATS
        offense.damage.SetBaseValue(offenseGroup.damage.GetValue() * increase);
        offense.attackSpeed.SetBaseValue(offenseGroup.attackSpeed.GetValue() * increase);
        offense.critChance.SetBaseValue(offenseGroup.critChance.GetValue() * increase);
        offense.critPower.SetBaseValue(offenseGroup.critPower.GetValue() * increase);
        offense.fireDamage.SetBaseValue(offenseGroup.fireDamage.GetValue() * increase);
        offense.iceDamage.SetBaseValue(offenseGroup.iceDamage.GetValue() * increase);
        offense.lightningDamage.SetBaseValue(offenseGroup.lightningDamage.GetValue() * increase);

        defense.evasion.SetBaseValue(defenseGroup.evasion.GetValue() * increase);

        // PENALTY STATS
        resources.maxHealth.SetBaseValue(resourceGroup.maxHealth.GetValue() * penalty);
        resources.healthRegen.SetBaseValue(resourceGroup.healthRegen.GetValue() * penalty);

        defense.armor.SetBaseValue(defenseGroup.armor.GetValue() * penalty);
        defense.lightningRes.SetBaseValue(defenseGroup.lightningRes.GetValue() * penalty);
        defense.fireRes.SetBaseValue(defenseGroup.fireRes.GetValue() * penalty);
        defense.iceRes.SetBaseValue(defenseGroup.iceRes.GetValue() * penalty);
    }
}
