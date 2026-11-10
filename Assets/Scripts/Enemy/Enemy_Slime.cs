using UnityEngine;

public class Enemy_Slime : Enemy, ICounterable
{
    [Header("Slime Specific")]
    [SerializeField] private int amountOfSlimesToCreate = 2;
    [SerializeField] private GameObject slimeToCreatePrefab;
    [SerializeField] private bool hasStunRecoveryAniamtion = true;

    private bool spawnedFromSplit;
    private Transform spawnTarget;

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");  
        deadState = new Enemy_SlimeDeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        anim.SetBool("hasStunRecovery", hasStunRecoveryAniamtion);
    }

    protected override void Start()
    {
        base.Start();

        if (spawnedFromSplit)
            ApplyRespwanVelocity();

        if (spawnedFromSplit && spawnTarget != null)
        {
            player = spawnTarget;

            HandleFlip(player.position.x - transform.position.x);
            stateMachine.Initialize(battleState);
        }
        else
        {
            stateMachine.Initialize(idleState);
        }
    }

    public void CreateSlimeInDead()
    {
        if (slimeToCreatePrefab == null || amountOfSlimesToCreate == 0)
            return;
        
        for (int i = 0; i < amountOfSlimesToCreate; ++i)
        {
            GameObject newSlime = Instantiate(slimeToCreatePrefab, transform.position, Quaternion.identity);

            Enemy_Slime child = newSlime.GetComponent<Enemy_Slime>();
            child.SetupSplitSpawn(this);
        }
    }

    public void SetupSplitSpawn(Enemy_Slime parent)
    {
        spawnedFromSplit = true;

        spawnTarget = parent.player != null
            ? parent.player
            : Player.instance != null ? Player.instance.transform : null;

        stats.AdjustStatSetup(parent.stats, 0.6f, 1.2f);
    }

    private void ApplyRespwanVelocity()
    {
        Vector2 velocity = new Vector2(stunnedPower.x * Random.Range(-1f, 1f), stunnedPower.y * Random.Range(1f, 2f));
        SetVelocity(velocity.x, velocity.y);
    }

    public void HandleCounter()
    {
        if (!CanBeCountered) return;
        
        stateMachine.ChangeState(stunnedState);
    }
}
