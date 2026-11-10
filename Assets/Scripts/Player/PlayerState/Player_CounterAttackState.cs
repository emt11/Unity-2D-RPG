using UnityEngine;

public class Player_CounterAttackState : PlayerState
{
    private Player_Combat playerCombat;
    private bool counteredSomebody;

    public Player_CounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        playerCombat = player.GetComponent<Player_Combat>();
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = playerCombat.GetCounterDuration();
        counteredSomebody = playerCombat.CounterAttackPerformered();
        anim.SetBool("counterAttackPerformed", counteredSomebody);
    }

    public override void Update()
    {
        base.Update();

        rb.linearVelocity = new Vector2(0, rb.linearVelocityY);

        if (triggerCalled) // 成功格挡完毕
            stateMachine.ChangeState(player.idleState);

        if (stateTimer < 0 && !counteredSomebody) // 没有成功格挡
            stateMachine.ChangeState(player.idleState);
    }
}
