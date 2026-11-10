using UnityEngine;

public class Player_BasicAttackState : PlayerState
{
    private float attackVelocityTimer;
    private float lastComboTime;
    private bool comboAttackQueued;
    private int attackDir;

    private const int FirstComboIndex = 1;
    private int comboLimit = 3;
    private int comboIndex = 1;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
            comboLimit = player.attackVelocity.Length;
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndexIfNeeded();
        attackDir = player.moveInput.x != 0 ? (int) player.moveInput.x : player.facingDir; // 保证攻击过程中能够改变攻击方向

        anim.SetInteger("basicAttackIndex", comboIndex);
        GenerateAttackVelocity();
        SyncAttackSpeed();
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        // 如果在同一帧中进行了连续攻击，我们不应该让它进入 idle ，而是直接进入下一个攻击
        if (input.Player.BasicAttack.WasPressedThisFrame())
            if (comboIndex < comboLimit) // 最后一个攻击动画停止 进入 idle 
                comboAttackQueued = true;
        
        if (triggerCalled)
            HandleExit();
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastComboTime = Time.time;
    }

    /// <summary>
    /// 如果当前攻击动画未结束，玩家进行了连击，应该直接进入下一个攻击动画
    /// 否则才进入 idle 
    /// </summary>
    private void HandleExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false); // 当前帧设置 false 
            player.EnterAttackStateWithDelay();
            
        }
        else
            stateMachine.ChangeState(player.idleState);
    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;
        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocityY);
    }

    private void GenerateAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];
        attackVelocityTimer = player.attactVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }

    private void ResetComboIndexIfNeeded()
    {
        if (comboIndex > comboLimit || Time.time > lastComboTime + player.comboResetTime)
            comboIndex = FirstComboIndex;
    }
}
