using UnityEngine;

public class Enemy_ReaperAttackState : EnemyState
{
    private Enemy_Reaper reaper;

    public Enemy_ReaperAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        reaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            if (reaper.ShouldTeleport())
                stateMachine.ChangeState(reaper.reaperTeleportState);
            else
                stateMachine.ChangeState(reaper.reaperBattleState);
        }
    }
}
