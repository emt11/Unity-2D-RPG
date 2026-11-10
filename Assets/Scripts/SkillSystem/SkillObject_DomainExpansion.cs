using UnityEngine;

public class SkillObject_DomainExpansion :  SkillObject_Base
{
    private Skill_DomainExpansion domainManager;

    private float duration;
    private float expandSpeed;
    private float slowDownPercent;

    private Vector3 targetScale;
    private bool isShrinking;

    public void SetupDomain(Skill_DomainExpansion domainManager)
    {
        this.domainManager = domainManager;
        float maxScale = domainManager.maxDomainSize;
        duration = domainManager.GetDomainDuration();
        slowDownPercent = domainManager.GetSlowDownPercent();
        expandSpeed = domainManager.expandSpeed;

        targetScale = Vector3.one * maxScale;
        Invoke(nameof(ShrinkDomain), duration);
    }

    private void Update()
    {
        HandleScaling();
    }

    private void HandleScaling()
    {
        float sizeDifference = Mathf.Abs(transform.localScale.x - targetScale.x);

        if (sizeDifference > 0.1f)
            transform.localScale = Vector2.Lerp(transform.localScale, targetScale, expandSpeed * Time.deltaTime);

        if (isShrinking && sizeDifference < 0.1f)
            TerminateDomain();
    }

    private void TerminateDomain()
    {
        domainManager.ClaerTargets();
        Destroy(gameObject);
    }

    private void ShrinkDomain()
    {
        isShrinking = true;
        targetScale = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        domainManager.AddTarget(enemy);
        if (enemy == null) return;
        enemy.SlowDownEntity(duration, slowDownPercent, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null) return;
        enemy.StopSlowDown();
    }
}
