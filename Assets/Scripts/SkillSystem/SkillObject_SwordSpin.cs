using UnityEngine;

public class SkillObject_SwordSpin : SkillObject_Sword
{
    private float maxDistance;
    private float maxDuration;
    private int attackPerSecond;
    private float attackTimer;

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        this.maxDistance = swordManager.maxDistance;
        this.maxDuration = swordManager.maxDuration;
        this.attackPerSecond = swordManager.attackPerSecond;
        anim?.SetTrigger("spin");
        Invoke(nameof(GetSwordBackToPlayer), maxDuration); // 指定时间自动收回
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        rb.simulated = false;
    }

    protected override void Update()
    {
        base.Update();
        HandleAttack();
        HandleStopping();
        HandleComeback();
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer < 0)
        {
            DamageEnemiesInRadius(transform, 1f);
            attackTimer = 1f / attackPerSecond;
        }
    }

    private void HandleStopping()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistance)
            rb.simulated = false;
    }
}
