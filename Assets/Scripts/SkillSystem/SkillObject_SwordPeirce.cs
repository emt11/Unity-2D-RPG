using UnityEngine;

public class SkillObject_SwordPeirce : SkillObject_Sword
{
    private int amountToPierce;

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        this.amountToPierce = swordManager.amountToPierce;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        --amountToPierce;
        DamageEnemiesInRadius(transform, 0.3f);
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");
        if (amountToPierce <= 0 || groundHit)
            StopSword(collision);
    }
}
