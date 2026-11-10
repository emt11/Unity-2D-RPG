using UnityEngine;

public class Enemy_AnimationTrigger : Entity_AnimationTriggers
{
    private Enemy enemy;
    private Enemy_VFX enemy_VFX;

    public override void Awake()
    {
        base.Awake();

        enemy = GetComponentInParent<Enemy>();
        enemy_VFX = GetComponentInParent<Enemy_VFX>();
    }

    private void SpecialAttackTrigger()
    {
        enemy.SpecialAttack();
    }

    private void EnableCounterWindow()
    {
        enemy_VFX.EnableAttackAlert(true);
        enemy.EnableCounterWindow(true);
    }

    private void DisEnableCounterWindow()
    {
        enemy_VFX.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);
    }
}
