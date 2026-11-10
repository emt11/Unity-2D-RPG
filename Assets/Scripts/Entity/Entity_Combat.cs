using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public event Action<float> OnDoingPhysicalDamage;

    private Entity_VFX vfx;
    private Entity_SFX sfx;
    private Entity_Stats stats;
    [SerializeField] private DamageScaleData basicAttackData;
    
    [Header("Target details")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] protected LayerMask whatIsTarget;

    public virtual void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        sfx = GetComponent<Entity_SFX>();
        stats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        bool wasHitOrAlive = false;
        foreach (var target in GetDetectedColliders(whatIsTarget))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null) 
                continue;

            AttackData attackData = stats.GetAttackData(basicAttackData);
            ElementalEffectData effectData = attackData.effectData;
            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            ElementType element = attackData.element;

            // 对 target 添加元素效果
            if (element != ElementType.None)
                target.GetComponent<Entity_StatusHandler>()?.ApplyStatusEffect(element, effectData);

            wasHitOrAlive = damageable.TakeDamage(physicalDamage, elementalDamage, element, transform);
            if (wasHitOrAlive) { // 闪避或者死亡不加攻击特效
                OnDoingPhysicalDamage?.Invoke(physicalDamage);
                vfx.CreateOnHitVFX(target.transform, attackData.isCrit, element); // 攻击特效
                sfx?.PlayAttackHit();
            }
        }

        if (!wasHitOrAlive) sfx?.PlayAttackMiss();
    }

    public void PerformAttackOnTarget(Transform target)
    {
        bool wasHitOrAlive = false;
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable == null) return;

        AttackData attackData = stats.GetAttackData(basicAttackData);
        ElementalEffectData effectData = attackData.effectData;
        float physicalDamage = attackData.physicalDamage;
        float elementalDamage = attackData.elementalDamage;
        ElementType element = attackData.element;

        // 对 target 添加元素效果
        if (element != ElementType.None)
            target.GetComponent<Entity_StatusHandler>()?.ApplyStatusEffect(element, effectData);

        wasHitOrAlive = damageable.TakeDamage(physicalDamage, elementalDamage, element, transform);
        if (wasHitOrAlive) { // 闪避或者死亡不加攻击特效
            OnDoingPhysicalDamage?.Invoke(physicalDamage);
            vfx.CreateOnHitVFX(target.transform, attackData.isCrit, element); // 攻击特效
            sfx?.PlayAttackHit();
        }

        if (!wasHitOrAlive) sfx?.PlayAttackMiss();
    }

    protected Collider2D[] GetDetectedColliders(LayerMask whatToDetect)
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatToDetect);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
