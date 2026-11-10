using UnityEngine;

public abstract class Player_AiredState : PlayerState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }    

    public override void Update()
    {
        base.Update();

        HandleAirMovement();
        HandleWallSlide();
        HandleAirAttack();
    }

    protected virtual void HandleWallSlide()
    {
        if (player.wallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }

    protected virtual void HandleAirAttack()
    {
        if (input.Player.BasicAttack.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpAttackState);
    }

    protected virtual void HandleAirMovement()
    {
        if (player.moveInput.x != 0)
            player.SetVelocity(player.moveInput.x * player.moveSpeed * player.inAirMoveMultiplier, rb.linearVelocityY);
    }
}
