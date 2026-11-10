using System.Collections.Generic;
using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{
    [SerializeField] private GameObject domainPrefab;

    [Header("Slow Down Upgrade")]
    [SerializeField] private float slowDownPercent = 0.8f;
    [SerializeField] private float slowDownDomainDuration = 5f;

    [Header("Shard Cast Upgrade")]
    [SerializeField] private int shardsToCast = 15;
    [SerializeField] private float shardCastDomainSlow = 1f;
    [SerializeField] private float shardCastDomainDuration = 8f;

    [Header("Time Echo Cast Upgrade")]
    [SerializeField] private int echoesToCast = 15;
    [SerializeField] private float echoCastDomainSlow = 1f;
    [SerializeField] private float echoCastDomainDuration = 8f;
    [SerializeField] private float healthToRestoreWithEcho = 0.05f;

    [Header("Domain Details")]
    public float maxDomainSize = 10f;
    public float expandSpeed = 3f;

    private float spellCastTimer;
    private float spellsPerSecond;
    private List<Enemy> trappedTargets = new List<Enemy>();
    private Transform currentTarget;

    public void CreateDomain()
    {
        spellsPerSecond = GetSpellToCast() / GetDomainDuration();
        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);
    }

    public void DoSpellCasting()
    {
        spellCastTimer -= Time.deltaTime;
        if (currentTarget == null) 
            currentTarget = FindTargetInDomain();

        if (currentTarget != null && spellCastTimer < 0)
        {
            CastSpell(currentTarget);
            spellCastTimer = 1 / spellsPerSecond;
            currentTarget = null;
        }
    }

    private void CastSpell(Transform target)
    {
        if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
        {
            Vector3 xOffset = Random.value < 0.5f ? new Vector2(1, 0) : new Vector2(-1, 0);
            skillManager.timeEcho.CreateTimeEcho(target.position + xOffset);
        }

        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
        {
            skillManager.shard.CreateRawShard(target, true);
        }
    }

    private Transform FindTargetInDomain()
    {
        trappedTargets.RemoveAll(target => target == null || target.health.isDead);
        if (trappedTargets.Count == 0) return null;
        int randomIndex = Random.Range(0, trappedTargets.Count);
        return trappedTargets[randomIndex].transform;
    }

    public float GetDomainDuration()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
            return slowDownDomainDuration;
        else if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            return shardCastDomainDuration;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
            return echoCastDomainDuration;

        return 0f;
    }

    public float GetSlowDownPercent()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
            return slowDownPercent;
        else if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            return shardCastDomainSlow;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
            return echoCastDomainSlow;

        return 0f;
    }

    public float GetSpellToCast()
    {
        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            return shardsToCast;
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
            return echoesToCast;
        
        return 0f;
    }

    public bool InstantDomain()
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpam
            && upgradeType != SkillUpgradeType.Domain_ShardSpam;
    }

    public void AddTarget(Enemy enemy)
    {
        trappedTargets.Add(enemy);
    }

    public void ClaerTargets()
    {
        foreach (var enemy in trappedTargets)
            enemy?.StopSlowDown();

        trappedTargets = new List<Enemy>();
    }
}
