using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Ice Blast", fileName = "Item Effect Data - Ice Blast On Taking Damage")]
public class ItemEffect_IceBlastOnTakingDamage : ItemEffect_DataSO
{
    [SerializeField] private ElementalEffectData effectData;
    [SerializeField] private float iceDamage;
    [SerializeField] private LayerMask whatIsEnemy;

    [Space]
    [SerializeField] private float healthPercentTrigger = 0.25f;
    [SerializeField] private float cooldown;
    private float lastTimeUsed;

    [Header("VFX Objects")]
    [SerializeField] private GameObject onHitVFX;
    [SerializeField] private GameObject iceBlastVFX;

    public override void ExecuteEffect()
    {
        bool notCooldown = Time.time > lastTimeUsed + cooldown;
        bool reachedThreshold = player.health.GetHealthPercent() <= healthPercentTrigger;
        if (notCooldown && reachedThreshold)
        {
            player.vfx.CreateEffectOf(iceBlastVFX, player.transform);
            DamageEnemiesWithIce();
            lastTimeUsed = Time.time;
        }
    }

    private void DamageEnemiesWithIce()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, 1.5f, whatIsEnemy);
        foreach (var enemy in enemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable == null)
                continue;

            Entity_StatusHandler statusHandler = enemy.GetComponent<Entity_StatusHandler>();
            statusHandler?.ApplyStatusEffect(ElementType.Ice, effectData);

            bool wasHitOrAlive = damageable.TakeDamage(0, iceDamage, ElementType.Ice, player.transform);
            if (wasHitOrAlive)
                player.vfx.CreateEffectOf(onHitVFX, enemy.transform);
        }
    }

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        lastTimeUsed = float.NegativeInfinity;
        player.health.OnTakingDamage += ExecuteEffect;
    }

    public override void UnSubscribe()
    {
        base.UnSubscribe();
        player.health.OnTakingDamage -= ExecuteEffect;
        player = null;
    }
}
