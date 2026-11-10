using System;
using UnityEngine;

/// <summary>
/// Shard 类技能对象：
/// 1. 触发
/// 2. shard 爆炸
/// </summary>
public class SkillObject_Shard : SkillObject_Base
{
    public event Action OnExplode;
    private Skill_Shard shardManager;

    [SerializeField] private GameObject vfxPrefab;
    
    private Transform target;
    private float moveSpeed;

    private void Update()
    {
        if (target == null) return;
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    public void MoveTowardsClosestTarget(float speed, Transform newTarget = null)
    {
        target = newTarget == null ? FindClosestTarget() : newTarget;
        moveSpeed = speed;
    }

    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;
        this.playerStats = shardManager.player.stats;
        this.damageScaleData = shardManager.damageScaleData;
        Invoke(nameof(Explode), shardManager.GetDetonateTime());
    }

    public void SetupShard(Skill_Shard shardManager, float detonationTime, bool canMove, float shardSpeed, Transform newTarget = null)
    {
        this.shardManager = shardManager;
        this.playerStats = shardManager.player.stats;
        this.damageScaleData = shardManager.damageScaleData;
        Invoke(nameof(Explode), detonationTime);
        if (canMove) 
            MoveTowardsClosestTarget(shardSpeed, newTarget);
    }

    public void Explode()
    {
        DamageEnemiesInRadius(targetCheck, checkRadius);
        GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = shardManager.player.vfx.GetElementColor(element);
        OnExplode?.Invoke(); // 发生爆炸则将技能置为冷却
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<Enemy>() == null) return;
        Explode();
    }
}
