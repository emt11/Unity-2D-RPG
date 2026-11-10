using System;
using UnityEngine;

public class Enemy_ArcherElfArrow : MonoBehaviour, ICounterable
{
    [SerializeField] private LayerMask whatIsTarget;

    private Collider2D col;
    private Rigidbody2D rb;
    private Entity_Combat combat;
    private Animator anim;

    public bool CanBeCountered => true;

    public void SetupArrow(float xVelocity, Entity_Combat combat)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        rb.linearVelocity = new Vector2(xVelocity, 0);
        this.combat = combat;

        if (rb.linearVelocityX < 0)
            transform.Rotate(0, 180, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsTarget) == 0)
            return;
        
        combat.PerformAttackOnTarget(collision.transform);
        StuckIntoTarget(collision.transform);
    }

    private void StuckIntoTarget(Transform target)
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;
        anim.enabled = false;

        transform.parent = target;

        Destroy(gameObject, 3f);
    }

    public void HandleCounter()
    {
        // 反方向射出
        rb.linearVelocity = new Vector2(-rb.linearVelocityX, 0f);
        transform.Rotate(0, 180, 0);

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        whatIsTarget = whatIsTarget | (1 << enemyLayer);
    }
}
