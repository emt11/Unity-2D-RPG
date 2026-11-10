using UnityEngine;

public class Enemy_SlimeDeadState : Enemy_DeadState
{
    private readonly Enemy_Slime enemySlime;
    private bool handledDeath;

    public Enemy_SlimeDeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemySlime = enemy as Enemy_Slime;
    }

    public override void Enter()
    {
        if (handledDeath)
            return;

        handledDeath = true;

        base.Enter();
        enemySlime.CreateSlimeInDead();
    }
}
