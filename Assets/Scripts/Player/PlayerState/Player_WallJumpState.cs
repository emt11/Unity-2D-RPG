using UnityEngine;

public class Player_WallJumpState : Player_AiredState
{
    private float controlTimer;

    public Player_WallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        controlTimer = player.wallJumpControlTime;
        player.SetVelocity(player.wallJumpForce.x * -player.facingDir, player.wallJumpForce.y); // 设置墙跳初始速度
    }

    public override void Update()
    {
        controlTimer -= Time.deltaTime;
        base.Update();
        if (rb.linearVelocityY < 0 && stateMachine.currentState != player.jumpAttackState)
            stateMachine.ChangeState(player.fallState);
    }

    protected override void HandleAirMovement()
    {
        if (controlTimer < 0)
            base.HandleAirMovement();
    }

    protected override void HandleWallSlide()
    {
        if (controlTimer < 0)
            base.HandleWallSlide();
    }
}
