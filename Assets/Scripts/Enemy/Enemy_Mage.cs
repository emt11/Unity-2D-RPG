using System.Collections;
using UnityEngine;

public class Enemy_Mage : Enemy, ICounterable
{
    public Enemy_MageRetreatState retreatState { get; private set; }
    public Enemy_MageSpellCastState spellCastState { get; private set; }

    [Header("Mage Specific")]
    public float retreatCooldown = 5f;
    public float retreatSpeed = 15f;
    public float maxRetreatDistance = 8f;
    [Space]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellStartPosition;
    [SerializeField] private int amountTocast = 3;
    [SerializeField] private Transform behindCollsionCheck;
    [SerializeField] private float spellCastCooldown = 0.3f;
    [SerializeField] private bool hasStunRecoveryAniamtion = true;

    public bool spellCastPerformed { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
        battleState = new Enemy_MageBattleState(this, stateMachine, "battle");
        retreatState = new Enemy_MageRetreatState(this, stateMachine, "battle");
        spellCastState = new Enemy_MageSpellCastState(this, stateMachine, "spellCast");

        anim.SetBool("hasStunRecovery", true);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void SpecialAttack()
    {
        StartCoroutine(CastSpellCo());
    }

    private IEnumerator CastSpellCo()
    {
        for (int i = 0; i < amountTocast; ++i)
        {
            Enemy_MageProjectile projectile = Instantiate(spellPrefab, spellStartPosition.position, Quaternion.identity).GetComponent<Enemy_MageProjectile>();
            projectile.SetupProjectile(player.transform, combat);
            yield return new WaitForSeconds(spellCastCooldown);
        }
        SetSpellCastPerformed(true);
    }

    public void SetSpellCastPerformed(bool spellCastPerformed) => this.spellCastPerformed = spellCastPerformed;

    public bool CanMoveBackwards()
    {
        bool detectedWall = Physics2D.Raycast(behindCollsionCheck.position, Vector2.right * -facingDir, 1.5f, whatIsGround);
        bool detectedGround = Physics2D.Raycast(behindCollsionCheck.position, Vector2.down, 1.5f, whatIsGround);

        return !detectedWall && detectedGround;
    }

    public void HandleCounter()
    {
        if (!CanBeCountered) return;
        
        stateMachine.ChangeState(stunnedState);
    }
}
