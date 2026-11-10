using UnityEngine;

public class SkillObject_Sword : SkillObject_Base
{
    protected Skill_SwordThrow swordManager;

    protected Transform playerTransform;
    protected bool shouldComeback;
    protected float comebackSpeed = 20f;
    protected float maxComebackDistance = 25f;

    protected virtual void Update()
    {
        transform.right = rb.linearVelocity;
        HandleComeback();
    }

    public virtual void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        rb.linearVelocity = direction;
        this.swordManager = swordManager;
        this.playerStats = swordManager.player.stats;
        damageScaleData = swordManager.damageScaleData;
        playerTransform = playerStats.transform.root;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        StopSword(collision);
        DamageEnemiesInRadius(transform, 1f);
    }

    protected void StopSword(Collider2D collision)
    {
        rb.simulated = false;
        transform.parent = collision.transform;

    }

    public void GetSwordBackToPlayer() => shouldComeback = true;

    protected void HandleComeback()
    {
        if (!shouldComeback)
            return;
        
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance > maxComebackDistance) 
            GetSwordBackToPlayer();

        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, comebackSpeed * Time.deltaTime);

        if (distance < 0.5f) 
            Destroy(gameObject);
    }
}
