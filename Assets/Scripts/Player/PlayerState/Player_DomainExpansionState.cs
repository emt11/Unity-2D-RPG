using System;
using UnityEngine;

public class Player_DomainExpansionState : PlayerState
{
    private Vector2 originalPosition;
    private float originalGravity;
    private float finalRiseDistance;

    private bool isLevitating;
    private bool createDomain;

    public Player_DomainExpansionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        originalPosition = player.transform.position;
        originalGravity = rb.gravityScale;
        finalRiseDistance = GetAvaliableRiseDistance();
        player.SetVelocity(0, player.riseSpeed);
        player.health.SetCanTakeDamage(false);
    }

    public override void Exit()
    {
        base.Exit();
        createDomain = false;
        player.health.SetCanTakeDamage(true);
    }

    public override void Update()
    {
        base.Update();

        if (!isLevitating)
        {
            bool reachedTarget = player.transform.position.y - originalPosition.y >= finalRiseDistance;
            bool stopped = rb.linearVelocityY <= 0;   // 重力上升段速度到不了 0，到 0 就是被撞停/撞反弹了
            if (reachedTarget || stopped)
                Levitate();
        }
        else
        {
            skillManager.domainExpansion.DoSpellCasting();
            if (stateTimer < 0)
            {
                isLevitating = false;
                rb.gravityScale = originalGravity;
                stateMachine.ChangeState(player.fallState);
            }
        }
    }

    private void Levitate()
    {
        isLevitating = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        stateTimer = skillManager.domainExpansion.GetDomainDuration();

        if (!createDomain)
        {
            createDomain = true;
            skillManager.domainExpansion.CreateDomain();
        }
    }

    private float GetAvaliableRiseDistance()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.up, player.riseMaxDistance, player.whatIsGround);
        
        return hit.collider != null ? hit.distance - 1f : player.riseMaxDistance;
    }
}
