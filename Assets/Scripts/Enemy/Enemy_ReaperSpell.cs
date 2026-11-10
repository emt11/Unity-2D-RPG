using UnityEngine;

public class Enemy_ReaperSpell : MonoBehaviour
{
    private Entity_Combat combat;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private Collider2D col;

    public void SetupSpell(Entity_Combat combat)
    {
        this.combat = combat;
        Destroy(gameObject, 2f);
    }

    private void EnableCollider() => col.enabled = true;
    private void DisableCollider() => col.enabled = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsTarget) == 0) return;

        combat.PerformAttackOnTarget(collision.transform);
        Destroy(gameObject, 1.5f);
    }
}
