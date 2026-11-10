using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.enabled = false;
        enemy.GetComponent<Collider2D>().enabled = false;
        rb.gravityScale = 12f;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 15);
        stateMachine.SwtichOffStateMachine();
        Object.Destroy(enemy.gameObject, enemy.destroyDelay);
    }
}
