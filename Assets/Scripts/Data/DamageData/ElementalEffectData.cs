using System;
using UnityEngine;

[Serializable]
public class ElementalEffectData
{
    public float chillDuration;
    public float chillSlowMultiplier;

    public float burnDuration;
    public float burnDamage;

    public float shockDuration;
    public float shockDamage;
    public float shockCharge;

    public ElementalEffectData(Entity_Stats entityStats, DamageScaleData damageScaleData)
    {
        chillDuration = damageScaleData.chillDuration;
        chillSlowMultiplier =  damageScaleData.chillSlowMultiplier;

        burnDuration = damageScaleData.burnDuration;
        burnDamage = entityStats.offense.fireDamage.GetValue() * damageScaleData.burnDamageScale;

        shockDuration = damageScaleData.shockDuration;
        shockDamage = entityStats.offense.lightningDamage.GetValue() * damageScaleData.shockDamageScale;
        shockCharge = damageScaleData.shockCharge;
    }
}
