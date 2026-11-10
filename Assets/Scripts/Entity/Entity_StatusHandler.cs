using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private ElementType currentEffect = ElementType.None;

    [Header("Shock effect details")]
    [SerializeField] private GameObject lightningStrikeVfx;
    [SerializeField] private float currentCharge;
    [SerializeField] private float maxCharge = 1f;
    private Coroutine shockCo;

    public void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
    }

    public void RemoveAllNegativeEffects()
    {
        StopAllCoroutines();
        currentEffect = ElementType.None;
        entityVfx.StopAllVfxs();
    }

    /// <summary>
    /// 应用状态效果
    /// </summary>
    /// <param name="element">元素类型</param>
    /// <param name="effectData">状态数据，不同技能/攻击 状态数据不同</param>
    public void ApplyStatusEffect(ElementType element, ElementalEffectData effectData)
    {
        if (element == ElementType.None) 
            return;
        else if (element == ElementType.Ice && CanBeApplied(element)) 
            ApplyChillEffect(effectData.chillDuration, effectData.chillSlowMultiplier);
        else if (element == ElementType.Fire && CanBeApplied(element))
            ApplyBurnEffect(effectData.burnDuration, effectData.burnDamage);
        else if (element == ElementType.Lightning && CanBeApplied(element))
            ApplyShockEffect(effectData.shockDuration, effectData.shockDamage, effectData.shockCharge);
    }

    private void ApplyShockEffect(float duration, float damage, float charge)
    {
        // Lightning 抗性 减少 charge 
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance);
        currentCharge += finalCharge;

        if (currentCharge >= maxCharge)
        {
            DoLightningStrike(damage);
            StopShockEffect();
            return;
        }

        if (shockCo != null)
            StopCoroutine(shockCo);

        shockCo = StartCoroutine(ShockEffectCo(duration));
    }

    private void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);
        entityHealth.ReduceHealth(damage);
    }

    private IEnumerator ShockEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusVfx(duration, currentEffect);
        
        yield return new WaitForSeconds(duration);

        StopShockEffect();
    }

    private void StopShockEffect()
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVfx.StopAllVfxs();
    }

    private void ApplyBurnEffect(float duration, float fireDamage)
    {
        // Fire 抗性 减少 fireDamage
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);
        float finalFireDamage = fireDamage * (1 - fireResistance);

        StartCoroutine(BurnEffectCo(duration, finalFireDamage));
    }

    private IEnumerator BurnEffectCo(float duration, float fireDamage)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);

        int burnPerSecond = 2; // 一秒钟 burn 两次
        int burnCount = Mathf.RoundToInt(burnPerSecond * duration);
        float damagePerTick = fireDamage / burnCount;
        float tickInterval = 1f / burnPerSecond;

        for (int i = 0; i < burnCount; ++i)
        {
            entityHealth.ReduceHealth(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;
    }

    private void ApplyChillEffect(float duration, float slowMultiplier)
    {
        // Ice 抗性 减少 Ice 元素效果持续时间
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);
        float finalDuration = duration * (1 - iceResistance);

        StartCoroutine(ChillEffectCo(finalDuration, slowMultiplier)); // 添加受击效果（闪烁冰冻以及缓速）
    }

    private IEnumerator ChillEffectCo(float duration, float slowMultiplier)
    {
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusVfx(duration, currentEffect);
        entity.SlowDownEntity(duration, slowMultiplier); // 对 target 减速

        yield return new WaitForSeconds(duration);
        
        currentEffect = ElementType.None;
    }

    private bool CanBeApplied(ElementType element)
    {
        if (element == ElementType.Lightning && currentEffect == ElementType.Lightning)
            return true;

        return currentEffect == ElementType.None;
    } 
}
