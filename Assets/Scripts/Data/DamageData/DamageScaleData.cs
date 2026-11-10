using UnityEngine;

[System.Serializable]
public class DamageScaleData
{
    [Header("Damage")]
    public float physicalScale = 1f;
    public float elementalScale = 1f;

    [Header("Chill")]
    public float chillDuration = 3f;
    public float chillSlowMultiplier = .2f;

    [Header("Burn")]
    public float burnDuration = 3f;
    public float burnDamageScale = 1f;

    [Header("Shock")]
    public float shockDuration = 3f;
    public float shockDamageScale = 1;
    public float shockCharge = .4f;
}
