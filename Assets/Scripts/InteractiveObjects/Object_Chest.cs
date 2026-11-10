using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable
{
    private Animator animator => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Entity_VFX entity_VFX => GetComponent<Entity_VFX>();
    private Entity_DropManager dropManager => GetComponent<Entity_DropManager>();

    [Header("Open details")]
    [SerializeField] private Vector2 knockback;
    [SerializeField] private bool canDropItems = true;

    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (!canDropItems) return false;

        canDropItems = false;
        dropManager.DropItems();
        entity_VFX.PlayOnDamageVfx();
        animator.SetBool("chestOpen", true);
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-100f, 100f);
        return true;
    }
}
