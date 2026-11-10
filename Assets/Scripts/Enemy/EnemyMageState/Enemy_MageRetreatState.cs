using UnityEngine;

public class Enemy_MageRetreatState : EnemyState
{
    private Enemy_Mage enemyMage;
    private Vector3 startPosition;
    private Transform player;
    private float retreatDirection;

    public Enemy_MageRetreatState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyMage = enemy as Enemy_Mage;
    }

    public override void Enter()
    {
        base.Enter();

        if (player == null)
            player = enemy.GetPlayerReference();

        startPosition = enemy.transform.position;
        retreatDirection = -DirectionToPlayer();
        rb.linearVelocity = new Vector2(enemyMage.retreatSpeed * retreatDirection, 0);

        enemy.HandleFlip(DirectionToPlayer());
        enemy.MakeUnTargetable(true);
        enemy.vfx.DoImageEchoEffect(1f);
    }

    public override void Update()
    {
        base.Update();

        float movedDistance = Mathf.Abs( enemy.transform.position.x - startPosition.x);

        if (movedDistance >= enemyMage.maxRetreatDistance || !enemyMage.CanMoveBackwards())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
            stateMachine.ChangeState(enemyMage.spellCastState);
            return;
        }

        rb.linearVelocity = new Vector2(enemyMage.retreatSpeed * retreatDirection, rb.linearVelocityY);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.vfx.StopImageEchoEffect();
        enemy.MakeUnTargetable(false);
    }

    protected float DirectionToPlayer()
    {
        if (player == null)
            return 0;

        return player.position.x > enemy.transform.position.x ? 1 : -1;        
    }
}
