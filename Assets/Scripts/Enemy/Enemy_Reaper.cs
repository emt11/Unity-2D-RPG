using System.Collections;
using UnityEngine;

public class Enemy_Reaper : Enemy, ICounterable
{
    public Enemy_ReaperBattleState reaperBattleState { get; private set; }
    public Enemy_ReaperAttackState reaperAttackState { get; private set; }
    public Enemy_ReaperTeleportState reaperTeleportState { get; private set; }
    public Enemy_ReaperSpellCastState reaperSpellCastState { get; private set; }

    [Header("Reaper specifics")]
    public float maxBattleIdleTime = 5f;

    [Header("Reaper spellCast")]
    [SerializeField] private GameObject spellCastPrefab;
    [SerializeField] private int amountToCast = 6;
    [SerializeField] private float spellCastRate = 1.2f;
    [SerializeField] private float spellCastCooldown = 10f;
    [SerializeField] private Vector2 playerOffsetPrediction;
    private float lastTimeCastedSpells = float.NegativeInfinity;
    public bool spellCastPerformed { get; private set; }
    public Player playerScript { get; private set; }

    [Header("Reaper Teletport")]
    [SerializeField] private BoxCollider2D arenaBounds;
    [SerializeField] private float offsetCenterY = 1.725f;
    [SerializeField] private float chanceToTeleport = 0.25f;
    private float defaultChanceToTeleport;

    public bool teleportTrigger { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        reaperAttackState = new Enemy_ReaperAttackState(this, stateMachine, "attack");
        reaperBattleState = new Enemy_ReaperBattleState(this, stateMachine, "battle");
        reaperTeleportState = new Enemy_ReaperTeleportState(this, stateMachine, "teleport");
        reaperSpellCastState = new Enemy_ReaperSpellCastState(this, stateMachine, "spellCast");

        battleState = reaperBattleState;
    }

    protected override void Start()
    {
        base.Start();

        arenaBounds.transform.parent = null;
        defaultChanceToTeleport = chanceToTeleport;
        stateMachine.Initialize(idleState);
    }

    public override void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == reaperSpellCastState) return;
        
        base.TryEnterBattleState(player);
    }

    public override void SpecialAttack()
    {
        StartCoroutine(CastSpellCo());
    }

    private IEnumerator CastSpellCo()
    {
        if (playerScript == null) playerScript = player.GetComponent<Player>();

        for (int i = 0; i < amountToCast; ++i)
        {
            bool playerMoving = playerScript.rb.linearVelocity.magnitude > 0;
            float xOffset = playerMoving ? playerOffsetPrediction.x * playerScript.facingDir : 0;
            Vector3 spellPosition = player.transform.position + new Vector3(xOffset, playerOffsetPrediction.y);

            Enemy_ReaperSpell spell = Instantiate(spellCastPrefab, spellPosition, Quaternion.identity).GetComponent<Enemy_ReaperSpell>();
            spell.SetupSpell(combat);

            yield return new WaitForSeconds(spellCastRate);
        }

        SetSpellCastPerformed(true);
    }

    public bool CanDoSpellCast() => Time.time > lastTimeCastedSpells + spellCastCooldown;

    public void SetSpellCastOnCooldown() => lastTimeCastedSpells = Time.time;

    public void SetTeleportTrigger(bool value) => teleportTrigger = value;

    public void SetSpellCastPerformed(bool spellCastPerformed) => this.spellCastPerformed = spellCastPerformed;

    public void HandleCounter()
    {
        if (!CanBeCountered) return;
        
        stateMachine.ChangeState(stunnedState);
    }

    public bool ShouldTeleport()
    {
        if (Random.value > chanceToTeleport)
        {
            chanceToTeleport += 0.05f;
            return false;
        }

        chanceToTeleport = defaultChanceToTeleport;
        return true;
    }

    public Vector3 FindTeleportPoint()
    {
        int maxAttempts = 10;
        float bossWithColliderHalf = col.bounds.size.x / 2;
        for (int i = 0; i < maxAttempts; ++i)
        {
            float randomX = Random.Range(arenaBounds.bounds.min.x + bossWithColliderHalf, arenaBounds.bounds.max.x - bossWithColliderHalf);
            Vector2 raycastPoint = new Vector2(randomX, arenaBounds.bounds.max.y);

            RaycastHit2D hit = Physics2D.Raycast(raycastPoint, Vector2.down, Mathf.Infinity, whatIsGround);
            if (hit.collider != null)
                return hit.point + new Vector2(0, offsetCenterY);
        }

        return transform.position;
    }
}
