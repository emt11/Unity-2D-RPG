using UnityEngine;

public class Player_DashState : PlayerState
{
    private int dashDir;
    private float originalGravityScale;
    
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.vfx.DoImageEchoEffect(player.dashDuration); // 实现残影 VFX 
        skillManager.dash.OnStartEffect(); // 技能 VFX 
        dashDir = player.moveInput.x != 0 ? (int) player.moveInput.x : player.facingDir;
        stateTimer = player.dashDuration;
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        player.health.SetCanTakeDamage(false);

        player.gameObject.layer = LayerMask.NameToLayer("UnTargetable");
    }

    public override void Update()
    {
        base.Update();
        CancelDashIfNeeded();
        player.SetVelocity(player.dashSpeed * dashDir, 0);
        if (stateTimer < 0)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else 
                stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        skillManager.dash.OnEndEffect();
        player.SetVelocity(0, 0);
        rb.gravityScale = originalGravityScale;
        player.health.SetCanTakeDamage(true);
        player.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public bool CancelDashIfNeeded()
    {
        if (player.wallDetected)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
        return true;
    }
}
