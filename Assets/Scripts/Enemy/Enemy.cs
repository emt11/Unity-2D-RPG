using System;
using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Quest Info")]
    public string questTargetId;

    public Enemy_IdleState idleState { get; protected set; }
    public Enemy_MoveState moveState { get; protected set; }
    public Enemy_AttackState attackState { get; protected set; }
    public Enemy_BattleState battleState { get; protected set; }
    public Enemy_DeadState deadState { get; protected set; }
    public Enemy_StunnedState stunnedState { get; protected set; }
    public Enemy_Health health { get; protected set; }
    public Entity_Stats stats { get; private set; }
    public Entity_Combat combat { get; private set; }
    public Entity_VFX vfx { get; private set; }
    
    [Header("Movement details")]
    public float idleTime = 2f;
    public float moveSpeed = 1.4f;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1f;

    [Header("Player detection")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] public float playerCheckDistance;
    public Transform player { get; protected set; }

    [Header("Battle details")]
    public float battleMoveSpeed = 3f;
    public float attackDistance = 2f;
    public float attackCooldowm;
    public float battleTimeDuration = 5f;
    public bool canChasePlayer = true;
    public float minRetreatDistance = 1f;
    public Vector2 retreatVelocity;

    [Header("Death details")]
    [Min(0f)] public float destroyDelay = 3f;

    [Header("Stunned details")]
    public Vector2 stunnedPower = new Vector2(7f, 7f);
    public float stunnedDuration = 1f;
    protected bool canBeStunned;

    public bool CanBeCountered { get => canBeStunned; }

    public float activeSlowMultiplier { get; private set; } = 1f;

    public void EnableCounterWindow(bool enable) => canBeStunned = enable;

    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;

    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;

    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Enemy_Health>();
        stats = GetComponent<Entity_Stats>();
        combat = GetComponent<Entity_Combat>();
        vfx = GetComponent<Entity_VFX>();
    }

    public void MakeUnTargetable(bool canBeTargeted)
    {
        if (canBeTargeted)
            gameObject.layer = LayerMask.NameToLayer("UnTargetable");
        else
            gameObject.layer = LayerMask.NameToLayer("Enemy");
    }   

    public virtual void SpecialAttack()
    {
        
    }

    // SlowDown：三个速度（moveSpeed, battleMoveSpeed, anim.speed）
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        activeSlowMultiplier = 1 - slowMultiplier;
        anim.speed *= activeSlowMultiplier;

        yield return new WaitForSeconds(duration);

        StopSlowDown();
    }

    public override void StopSlowDown()
    {
        activeSlowMultiplier = 1f;
        anim.speed = 1f;
        base.StopSlowDown();
    }

    public override void EntityDeath()
    {
        base.EntityDeath();
        stateMachine.ChangeState(deadState);
    }

    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }

    public virtual void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;

        this.player = player;
        stateMachine.ChangeState(battleState);
    }

    public Transform GetPlayerReference()
    {
        if (player == null) 
            player = PlayerDetected().transform;
            
        return player;
    }

    public RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround);
        // 如果没有检测到结果 或者 最近一个物体不是玩家
        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;
        return hit;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + playerCheckDistance * facingDir, playerCheck.position.y, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + attackDistance * facingDir, playerCheck.position.y, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + minRetreatDistance * facingDir, playerCheck.position.y, 0));
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }
}
