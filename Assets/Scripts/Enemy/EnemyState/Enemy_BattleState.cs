using System;
using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private Transform lastTarget;
    private float lastTimeWasInBattle;
    protected float lastTimeAttacked = float.NegativeInfinity;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        if (player == null)
            player = enemy.GetPlayerReference();

        if (shouldRetreat())
            HandleRetreat();

        UpdateBattleTimer();
    }

    protected virtual void HandleRetreat()
    {
        ShortRetreat();
    }

    protected void ShortRetreat()
    {
        float x = enemy.retreatVelocity.x * enemy.activeSlowMultiplier * -DirectionToPlayer();
        float y = enemy.retreatVelocity.y;

        rb.linearVelocity = new Vector2(x, y);
        enemy.HandleFlip(DirectionToPlayer());
    }

    public override void Update()
    {
        base.Update();

        if (HandlePositioning())
        {
            UpdateBattleTimer();
            return;
        }

        if (enemy.PlayerDetected())
        {
            updateTargetIfNeeded();
            UpdateBattleTimer();
        }

        if (BattleTimeIsOver())
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (WithInAttackRange() && enemy.PlayerDetected() && AttackWithoutCooldown())
        {
            lastTimeAttacked = Time.time;
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        float speed = enemy.canChasePlayer ? enemy.GetBattleMoveSpeed() : 0f;

        enemy.SetVelocity(speed * DirectionToPlayer(), rb.linearVelocityY);
    }

    protected void updateTargetIfNeeded()
    {
        if (!enemy.PlayerDetected()) 
            return;

        Transform newTarget = enemy.PlayerDetected().transform;
        if (newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
            
    }

    public bool AttackWithoutCooldown() => Time.time > lastTimeAttacked + enemy.attackCooldowm;

    protected bool shouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

    protected bool BattleTimeIsOver() => lastTimeWasInBattle + enemy.battleTimeDuration < Time.time;

    protected void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;

    protected bool WithInAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    protected float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;
        
        return Math.Abs(player.position.x - enemy.transform.position.x);
    }

    protected float DirectionToPlayer()
    {
        if (player == null)
            return 0;

        return player.position.x > enemy.transform.position.x ? 1 : -1;        
    }

    protected virtual bool HandlePositioning() => false;
}
