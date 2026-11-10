using UnityEngine;

public class AttackData
{
    public float physicalDamage;
    public float elementalDamage;
    public bool isCrit;
    public ElementType element;
    public ElementalEffectData effectData;

    public AttackData(Entity_Stats stats, DamageScaleData damageScaleData)
    {
        physicalDamage = stats.GetPhysicalDamage(out isCrit, damageScaleData.physicalScale);
        elementalDamage = stats.GetElementalDamage(out element, damageScaleData.elementalScale);
        effectData = new ElementalEffectData(stats, damageScaleData);
    }
}
