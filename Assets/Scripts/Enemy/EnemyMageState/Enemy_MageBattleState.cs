using UnityEngine;

public class Enemy_MageBattleState : Enemy_BattleState
{
    private Enemy_Mage enemyMage;
    private float lastTimeRetreated = float.NegativeInfinity;

    public Enemy_MageBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyMage = enemy as Enemy_Mage;
    }

    protected override void HandleRetreat()
    {
        if (!CanUseRetreat()) return;
        
        lastTimeRetreated = Time.time;
        stateMachine.ChangeState(enemyMage.retreatState);
    }

    private bool CanUseRetreat() => Time.time > lastTimeRetreated + enemyMage.retreatCooldown;
}
