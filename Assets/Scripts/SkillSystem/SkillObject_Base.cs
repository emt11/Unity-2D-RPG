using UnityEngine;

/// <summary>
/// 技能通用对象：
/// 1. 技能范围
/// 2. 显示技能范围
/// 3. 范围内敌人受到伤害
/// </summary>
public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] private GameObject onHixVfx;
    [Space]
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1f;

    protected bool targetToHit;
    protected Entity_Stats playerStats;
    protected DamageScaleData damageScaleData;
    protected ElementType element;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Transform lastTarget;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected void DamageEnemiesInRadius(Transform t, float radius)
    {
        foreach (var target in GetEnemiesAround(t, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null) continue;

            AttackData attackData = playerStats.GetAttackData(damageScaleData);
            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            ElementType element = attackData.element;
            
            targetToHit = damageable.TakeDamage(physicalDamage, elementalDamage, element, transform);

            if (element != ElementType.None)
                target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(element, new ElementalEffectData(playerStats, damageScaleData));

            if (targetToHit)
            {
                lastTarget = target.transform;
                Instantiate(onHixVfx, target.transform.position, Quaternion.identity);
            }

            this.element = element;
        }
    }

    protected Transform FindClosestTarget()
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;
        foreach (var enemy in GetEnemiesAround(transform, 10f))
        {
            float currentDistance = Vector2.Distance(transform.position, enemy.transform.position);
            if (currentDistance < closestDistance)
            {
                target = enemy.transform;
                closestDistance = currentDistance;
            }
        }
        return target;
    }

    protected Collider2D[] GetEnemiesAround(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, whatIsEnemy);
    }

    protected void OnDrawGizmos()
    {
        if (targetCheck == null) return;
        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}
