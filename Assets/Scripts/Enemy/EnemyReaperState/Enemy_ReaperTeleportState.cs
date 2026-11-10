using UnityEngine;

public class Enemy_ReaperTeleportState : EnemyState
{
    private Enemy_Reaper reaper;

    public Enemy_ReaperTeleportState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        reaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.MakeUnTargetable(true);
    }

    public override void Update()
    {
        base.Update();

        if (reaper.teleportTrigger)
        {
            // 消失动画播放完毕，进行传送！
            reaper.transform.position = reaper.FindTeleportPoint();
            reaper.SetTeleportTrigger(false);
        }
        
        if (triggerCalled)
        {
            if (reaper.CanDoSpellCast())
                stateMachine.ChangeState(reaper.reaperSpellCastState);
            else
                stateMachine.ChangeState(reaper.reaperBattleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.MakeUnTargetable(false);
    }
}
