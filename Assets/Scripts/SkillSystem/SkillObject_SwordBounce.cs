using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    [SerializeField] public float bounceSpeed;
    public int bounceCounts;

    private Collider2D[] enemyTargets;
    private Transform nextTarget;
    private List<Transform> selectedBefore = new List<Transform>();

    protected override void Update()
    {
        HandleComeback();
        HandleBounce();
    }

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        anim.SetTrigger("spin");
        this.bounceSpeed = swordManager.bounceSpeed;
        this.bounceCounts = swordManager.bounceCounts;
    }

    // 当投掷的剑被触发时，检测第一个目标的周围的敌人
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyTargets == null)
        {
            enemyTargets = GetEnemiesAround(transform, 10f);
            rb.simulated = false;
        }

        DamageEnemiesInRadius(transform, 1f);

        if (enemyTargets.Length <= 1 || bounceCounts == 0)
            GetSwordBackToPlayer();
        else 
            nextTarget = GetNextTarget();
    }

    private void HandleBounce()
    {
        if (nextTarget == null) 
            return;

        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, nextTarget.position) < 0.75f)
        {
            DamageEnemiesInRadius(transform, 1f);
            BounceToNextTarget();
            if (bounceCounts == 0 || nextTarget == null)
            {
                nextTarget = null;
                GetSwordBackToPlayer();
            }
        }
    }

    // 弹射到下一个敌人
    private void BounceToNextTarget()
    {
        --bounceCounts;
        nextTarget = GetNextTarget();
    }

    // 寻找下一个敌人
    private Transform GetNextTarget()
    {
        List<Transform> validTargets = GetValidTargets();
        int randomIndex = Random.Range(0, validTargets.Count);
        Transform nextTarget = validTargets[randomIndex];
        selectedBefore.Add(nextTarget);

        return nextTarget;
    }

    // 找到所有可以被弹射的敌人
    private List<Transform> GetValidTargets()
    {
        List<Transform> vaildTargets = new List<Transform>();
        List<Transform> aliveTargets = GetAliveTargets();
        // 找到所有存活并没有弹射过的敌人。注意：当敌人死亡时，enemyTargets[i] == null
        foreach (var target in enemyTargets)
        {
            if (target != null && !selectedBefore.Contains(target.transform))
            {
                vaildTargets.Add(target.transform);
            }
        }
        if (vaildTargets.Count > 0)
            return vaildTargets;
        else
        {
            selectedBefore.Clear();
            return aliveTargets;
        }
    }

    private List<Transform> GetAliveTargets()
    {
        List<Transform> aliveTargets = new List<Transform>();
        // 找到所有存活并没有弹射过的敌人。注意：当敌人死亡时，enemyTargets[i] == null
        foreach (var target in enemyTargets)
            if (target != null)
                aliveTargets.Add(target.transform);
        
        return aliveTargets;
    }
}
