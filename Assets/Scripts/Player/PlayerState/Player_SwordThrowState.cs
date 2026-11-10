using UnityEngine;

public class Player_SwordThrowState : PlayerState
{
    private Camera mainCamera;

    public Player_SwordThrowState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillManager.swordThrow.EnableDots(true);

        if (mainCamera != Camera.main)
            mainCamera = Camera.main;
    }

    public override void Update()
    {
        base.Update();

        
        Vector2 dirToMouse = DirectionToMouse();
        
        // 设置玩家转向
        player.HandleFlip(dirToMouse.x);
        player.SetVelocity(0, rb.linearVelocityY);
        skillManager.swordThrow.PredictTrajectory(dirToMouse);

        if (input.Player.BasicAttack.WasPressedThisFrame())
        {
            anim.SetBool("swordThrowPerformed", true);
            skillManager.swordThrow.EnableDots(false);
            skillManager.swordThrow.ConfirmTrajectory(dirToMouse);
        }

        if (input.Player.RangeAttack.WasReleasedThisFrame() || triggerCalled) // 手动释放或到达动画最后帧
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("swordThrowPerformed", false);
        skillManager.swordThrow.EnableDots(false);
    }

    private Vector2 DirectionToMouse()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 worldMousePostion = mainCamera.ScreenToWorldPoint(player.mousePosition);
        Vector2 direction = worldMousePostion - playerPosition;

        return direction.normalized;
    }
}
