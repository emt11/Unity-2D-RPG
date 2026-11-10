using UnityEngine;

public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;
    protected Player_SkillManager skillManager;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;
        this.anim = player.anim;
        this.rb = player.rb;
        this.input = player.input;
        this.stats = player.stats;
        this.skillManager = player.skillManager;
    }

    public override void Update()
    {
        base.Update();

        if (input.Player.Dash.WasPressedThisFrame() && canDash())
        {
            stateMachine.ChangeState(player.dashState);
            skillManager.dash.SetSkillOnCoolDown(); // 进入冷却
        }


        Skill_DomainExpansion domainExpansion = skillManager.domainExpansion;
        if (input.Player.UltimateSpell.WasPressedThisFrame() && domainExpansion.CanUseSkill())
        {
            if (domainExpansion.InstantDomain())
            {
                domainExpansion.CreateDomain();
            }
            else
            {
                stateMachine.ChangeState(player.domainExpansionState);
            }
            domainExpansion.SetSkillOnCoolDown();
        }
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        anim.SetFloat("yVolecity", rb.linearVelocityY);
    }

    private bool canDash()
    {
        // if (player.wallDetected)
        //     return false;

        if (!skillManager.dash.CanUseSkill())
            return false;

        if (stateMachine.currentState == player.dashState 
            || stateMachine.currentState == player.wallSlideState 
            || stateMachine.currentState == player.domainExpansionState)   
            return false;

        return true;
    }
}
