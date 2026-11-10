using UnityEngine;

public class Enemy_ReaperSpellCastState : EnemyState
{
    private Enemy_Reaper reaper;

    public Enemy_ReaperSpellCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        reaper = enemy as Enemy_Reaper;        
    }

    public override void Enter()
    {
        base.Enter();

        reaper.SetVelocity(0, 0);
        reaper.SetSpellCastPerformed(false);
        reaper.SetSpellCastOnCooldown();
    }

    public override void Update()
    {
        base.Update();

        if (reaper.spellCastPerformed)
            anim.SetBool("spellCast_performed", true);

        if (triggerCalled)
            stateMachine.ChangeState(reaper.reaperBattleState);
    }

    public override void Exit()
    {
        base.Exit();
        
        anim.SetBool("spellCast_performed", false);
    }
}
