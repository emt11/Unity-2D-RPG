using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;
    private Player_QuestManager questManager;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
        questManager = Player.instance.questManager;
    }

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (!canTakeDamage) return false;

        bool wasHitOrAlive = base.TakeDamage(damage, elementalDamage, element, damageDealer);
        if (!wasHitOrAlive)
            return false;
        
        if (damageDealer.CompareTag("Player"))
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }

    protected override void Die()
    {
        base.Die();
        questManager.AddProcess(enemy.questTargetId, 1);
    }
}
