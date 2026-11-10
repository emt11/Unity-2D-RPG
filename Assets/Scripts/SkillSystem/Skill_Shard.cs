using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Shard 类技能对象
/// </summary>
public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    private Entity_Health entity_Health;

    [SerializeField] private GameObject shardPrefab;
    [SerializeField] public float detonationTime = 2f;

    [Header("Moving Shard Upgrade")]
    [SerializeField] private float shardSpeed = 7f;

    [Header("Multicast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private int currentCharges;
    [SerializeField] private bool isReCharging;

    [Header("Teleport Shard Upgrade")]
    [SerializeField] private float shardExistDuration = 10f;

    private float savedHealthPercent;

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
        entity_Health = GetComponentInParent<Entity_Health>();
    }

    public override void TryUseSkills()
    {
        if (!CanUseSkill())
            return;

        if (Unlocked(SkillUpgradeType.Shard))
            HandleShardRegular();

        if (Unlocked(SkillUpgradeType.Shard_MoveToEnemy))
            HandleShardMoving();

        if (Unlocked(SkillUpgradeType.Shard_Multicast))
            HandleShardMultiCast();

        if (Unlocked(SkillUpgradeType.Shard_Teleport))
            HandleShardTeleport();

        if (Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            HandleShardTeleportHpRewind();
    }

    private void HandleShardTeleportHpRewind()
    {
        if (currentShard == null)
        {
            CreateShard();
            savedHealthPercent = entity_Health.GetHealthPercent();
        }
        else
        {
            entity_Health.SetHealthPercent(savedHealthPercent);
            SwapPlayerAndShard();
            SetSkillOnCoolDown();
        }
    }

    /// <summary>
    /// 第一次触发技能：创建 shard 
    /// 第二次触发技能：交换位置并进入冷却
    /// </summary>
    private void HandleShardTeleport()
    {
        if (currentShard == null)
            CreateShard();
        else
        {
            SwapPlayerAndShard();
            SetSkillOnCoolDown();
        }
    }

    private void SwapPlayerAndShard()
    {
        Vector2 shardPosition = currentShard.transform.position;
        Vector2 playerPosition = player.transform.position;
        currentShard.transform.position = playerPosition;
        currentShard.Explode(); // 交换位置后立即爆炸，防止一个技能持续时间内多次交换位置
        player.TeleportPosition(shardPosition);
    }

    // TODO 感觉会有并发问题？
    private void HandleShardMultiCast()
    {
        if (currentCharges <= 0) return;
        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardSpeed);
        --currentCharges;
        if (!isReCharging)
            StartCoroutine(ShardRecharge());
    }

    private IEnumerator ShardRecharge()
    {
        isReCharging = true;
        if (currentCharges < maxCharges)
        {
            yield return new WaitForSeconds(cooldown);
            ++currentCharges;
        }
        isReCharging = false;
    }

    private void HandleShardMoving()
    {
        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardSpeed);
        SetSkillOnCoolDown();
    }

    private void HandleShardRegular()
    {
        CreateShard();
        SetSkillOnCoolDown();
    }

    public void CreateShard()
    {
        if (upgradeType == SkillUpgradeType.None) return;
        
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetupShard(this);
        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            currentShard.OnExplode += ForceOnCoolDown;
    }

    public void CreateRawShard(Transform target = null, bool shardsCanMove = false)
    {
        // TODO 这里如果技能树越深，要写的就越多，需要优化
        bool canMove = shardsCanMove || Unlocked(SkillUpgradeType.Shard_MoveToEnemy) || Unlocked(SkillUpgradeType.Shard_Multicast);
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        shard.GetComponent<SkillObject_Shard>().SetupShard(this, detonationTime, canMove, shardSpeed, target);
    }

    public float GetDetonateTime()
    {
        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
            return shardExistDuration;

        return detonationTime;
    }

    private void ForceOnCoolDown()
    {
        if (OnCooldown()) 
            return;

        SetSkillOnCoolDown();
    }
}
