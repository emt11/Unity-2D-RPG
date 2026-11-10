using UnityEngine;

public class Enemy_ReaperBattleState : Enemy_BattleState
{
    private Enemy_Reaper reaper;

    public Enemy_ReaperBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        reaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = reaper.maxBattleIdleTime;
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        if (stateTimer < 0)
            stateMachine.ChangeState(reaper.reaperTeleportState);

        if (enemy.PlayerDetected())
        {
            updateTargetIfNeeded();
            UpdateBattleTimer();
        }

        if (WithInAttackRange() && enemy.PlayerDetected() && AttackWithoutCooldown())
        {
            lastTimeAttacked = Time.time;
            stateMachine.ChangeState(reaper.reaperAttackState);
            return;
        }

        float xVelocity = enemy.canChasePlayer ? enemy.GetBattleMoveSpeed() : 0.0001f;
        if (!enemy.groundDetected)
            xVelocity = 0.0001f;

        enemy.SetVelocity(xVelocity * DirectionToPlayer(), rb.linearVelocityY);
    }
}
