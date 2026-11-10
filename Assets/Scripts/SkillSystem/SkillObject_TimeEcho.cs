using UnityEngine;

public class SkillObject_TimeEcho : SkillObject_Base
{
    private Transform playerTransform;
    private Skill_TimeEcho echoManager;
    private TrailRenderer wispTrail;
    private bool shouldMoveToPlayer;
    private Entity_Health playerHealth;
    private Player_SkillManager skillManager;
    private Entity_StatusHandler statusHandler;
    private SkillObject_Health echoHealth;

    [SerializeField] private float wispMoveSpeed = 15f;
    [SerializeField] GameObject onDeathVfx;
    [SerializeField] private LayerMask whatIsGround;
    
    public int maxAttacks { get; private set; }

    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;
        this.playerStats = echoManager.player.stats;
        this.damageScaleData = echoManager.damageScaleData;
        playerTransform = echoManager.transform.root;
        playerHealth = echoManager.player.health;
        skillManager = echoManager.player.skillManager;
        statusHandler = echoManager.player.statusHandler;
        echoHealth = GetComponent<SkillObject_Health>();

        Invoke(nameof(HandleDeath), echoManager.GetTimeEchoDuration());

        maxAttacks = echoManager.GetMaxAttacks();
        anim.SetBool("canAttack", maxAttacks > 0);

        FlipToTarget();
        
        wispTrail = GetComponentInChildren<TrailRenderer>();
        wispTrail.gameObject.SetActive(false);
    }

    private void FlipToTarget()
    {
        Transform target = FindClosestTarget();
        if (target == null) return;
        if (transform.position.x > target.position.x)
            transform.Rotate(0, 180, 0);
    }

    private void Update()
    {
        if (shouldMoveToPlayer)
            HandleWispMovement();
        else
        {
            anim.SetFloat("yVelocity", rb.linearVelocityY);
            StopHorizontalMovement();
        }
    }
    
    private void HandleWispMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, wispMoveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
        {
            HandleTouchPlayer();
            Destroy(gameObject);
        }
    }

    private void HandleTouchPlayer()
    {
        // Heal
        float healAmount = echoHealth.lastDamageTaken * echoManager.GetDamagePercentHealed();
        playerHealth.IncreaseHealth(healAmount);

        // Reduce cooldown
        float cooldownToReduce = echoManager.GetCooldownReducedInSeconds();
        skillManager.ReduceAllSKillCooldownBy(cooldownToReduce);

        // Remove all negative effects
        if (echoManager.CanRemoveAllNegativeEffects())
            statusHandler.RemoveAllNegativeEffects();
    }
    
    public void performAttack()
    {
        DamageEnemiesInRadius(targetCheck, 1f);

        if (!targetToHit)
            return;

        bool canDuplicate = Random.value < echoManager.GetDuplicateChance();
        if (canDuplicate)
        {
            float xOffset = transform.position.x < lastTarget.position.x ? 1 : -1;
            echoManager.CreateTimeEcho(lastTarget.position + new Vector3(xOffset, 0, 0));
        }
    }

    public void HandleDeath()
    {
        Instantiate(onDeathVfx, transform.position, Quaternion.identity);
        
        if (echoManager.ShouldBeWisp())
            TurnIntoWisp();
        else
            Destroy(gameObject);

    }

    private void TurnIntoWisp()
    {
        shouldMoveToPlayer = true;
        anim.gameObject.SetActive(false); // 不显示动画
        wispTrail.gameObject.SetActive(true);
        rb.simulated = false; // 防止撞墙
    }

    private void StopHorizontalMovement()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, whatIsGround);
        if (hit.collider != null)
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
    }
}
