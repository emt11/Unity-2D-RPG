using UnityEngine;

public class Enemy_ArcherElfBattleState : Enemy_BattleState
{
    private bool reachedDeadEnd;
    private bool waitingForLanding;

    public Enemy_ArcherElfBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        reachedDeadEnd = false;
        waitingForLanding = rb.linearVelocityY > 0.01f;
    }

    public override void Update()
    {
        if (waitingForLanding)
        {
            if (rb.linearVelocityY > 0f || !enemy.groundDetected)
            {
                UpdateBattleTimer();
                return;
            }

            waitingForLanding = false;
        }

        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        if (enemy.PlayerDetected())
        {
            updateTargetIfNeeded();
            UpdateBattleTimer();
        }

        if (BattleTimeIsOver())
        {
            StopMoving();
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (!WithInAttackRange())
        {
            enemy.HandleFlip(DirectionToPlayer());
            enemy.HandleCollisionDetection();

            if (enemy.canChasePlayer && enemy.groundDetected && !enemy.wallDetected)
                enemy.SetVelocity(enemy.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocityY);
            else
                StopMoving();

            return;
        }

        if (AttackWithoutCooldown())
        {
            // 冷却完成：停止后撤，面向玩家。
            StopMoving();
            enemy.HandleFlip(DirectionToPlayer());

            if (WithInAttackRange() && enemy.PlayerDetected())
            {
                lastTimeAttacked = Time.time;
                stateMachine.ChangeState(enemy.attackState);
            }

            return;
        }

        // 以下只在攻击冷却期间执行
        bool shouldWalkAway = !reachedDeadEnd && DistanceToPlayer() < enemy.attackDistance * 0.8f;

        if (shouldWalkAway)
        {
            // 先背对玩家，再检查移动方向
            float moveDirection = -DirectionToPlayer();

            enemy.HandleFlip(moveDirection);
            enemy.HandleCollisionDetection();

            if (enemy.wallDetected || !enemy.groundDetected)
            {
                reachedDeadEnd = true;
            }
            else
            {
                enemy.SetVelocity(enemy.GetBattleMoveSpeed() * moveDirection, rb.linearVelocityY);
                return;
            }
        }

        // 遇到死路或达到距离上限：原地等待冷却
        StopMoving();
        enemy.HandleFlip(DirectionToPlayer());
    }

    private void StopMoving()
    {
        enemy.SetVelocity(0f, rb.linearVelocityY);
    }
}