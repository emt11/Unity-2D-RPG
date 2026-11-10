using System;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamageable
{
    public event Action OnTakingDamage;
    public event Action OnHealthUpdate;

    private Slider healthBar;
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    private Entity_DropManager dropManager;

    public bool isDead { get; private set; }
    public float lastDamageTaken { get; private set; }
    protected bool canTakeDamage = true;

    [SerializeField] protected float currentHealth;

    [Header("Health Regen")]
    [SerializeField] private float regenInterval = 1f;
    [SerializeField] private bool canRegenerateHealth = true;
    
    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Heavy Damage Knockback")]
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f, 7f);
    [SerializeField] private float heavyKnockbackDuaration = 0.3f;
    [SerializeField] private float heavyKnockbackThreshold = 0.3f;

    protected virtual void Awake()
    {
        healthBar = GetComponentInChildren<Slider>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        dropManager = GetComponent<Entity_DropManager>();
    }

    protected virtual void Start()
    {
        SetupHealth();
    }

    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead || AttackEvaded() || !canTakeDamage)
            return false;

        Entity_Stats attackerStat = damageDealer.GetComponent<Entity_Stats>();
        
        // ArmorReduction
        float armorReduction = attackerStat != null ? attackerStat.GetArmorReduction() : 0;
        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0;
        float physicalDamageTaken = damage * (1 - mitigation);

        // ElementResistance
        float elementalResistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;
        float elementalDamageTaken = elementalDamage * (1 - elementalResistance);

        float totalDamageTaken = physicalDamageTaken + elementalDamageTaken;
        
        // 受击处理
        entity?.StartKnockbackCo(CalculateKnockback(physicalDamageTaken, damageDealer), CalculateKnockbackDuration(physicalDamageTaken)); // 受击后退
        entityVfx?.PlayOnDamageVfx(); // 受击视觉效果（变白） TODO 可以考虑移动到 ReduceHp 

        ReduceHealth(totalDamageTaken);
        lastDamageTaken = totalDamageTaken; // 用于技能治疗(Wisp)
        OnTakingDamage?.Invoke();
        
        return true;
    }

    private void SetupHealth()
    {
        if (entityStats == null)
            return;

        currentHealth = entityStats.GetMaxHealth();
        OnHealthUpdate += UpdateHealthBar;
        
        UpdateHealthBar();
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    private void RegenerateHealth()
    {
        if (!canRegenerateHealth)
            return;
        
        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }

    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();

    public void SetHealthPercent(float percent) 
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp01(percent);
        OnHealthUpdate?.Invoke();
    }

    public void IncreaseHealth(float healAmount)
    {
        if (isDead) 
            return;
        
        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.resources.maxHealth.GetValue();

        currentHealth = Mathf.Min(newHealth, maxHealth);
        OnHealthUpdate?.Invoke();
    }

    public void ReduceHealth(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        OnHealthUpdate?.Invoke();

        if (currentHealth <= 0)
            Die();
    }

    private bool AttackEvaded()
    {
        if (entityStats == null)
            return false;
        else
            return UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();
    }

    public float GetCurrentHealth() => currentHealth;

    private void UpdateHealthBar()
    {
        if (healthBar == null && !healthBar.transform.parent.gameObject.activeSelf)
            return;
        
        healthBar.value = currentHealth / entityStats.GetMaxHealth();
    }

    public void EnableHealthBar(bool enable) => healthBar?.transform.parent.gameObject.SetActive(enable);

    protected virtual void Die()
    {
        isDead = true;
        entity?.EntityDeath();
        dropManager?.DropItems();
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x < damageDealer.position.x ? -1 : 1;
        Vector2 knockback = isHeavyKnockback(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x *= direction;

        return knockback;
    }

    private float CalculateKnockbackDuration(float damage) => isHeavyKnockback(damage) ? heavyKnockbackDuaration : knockbackDuration;

    private bool isHeavyKnockback(float damage)
    {
        if (entityStats == null)
            return false;
        else 
            return damage / entityStats.GetMaxHealth() > heavyKnockbackThreshold;
    } 
        
}
