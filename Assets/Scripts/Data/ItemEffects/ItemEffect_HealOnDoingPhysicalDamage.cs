using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Heal On Doding Damage", fileName = "Item Effect Data - Heal On Doding Physical Damage")]
public class ItemEffect_HealOnDoingPhysicalDamage : ItemEffect_DataSO
{
   [SerializeField] private float percentHealedAttack = 0.2f;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.combat.OnDoingPhysicalDamage += HealOnDoingDamage;
    }

    public override void UnSubscribe()
    {
        base.UnSubscribe();
        player.combat.OnDoingPhysicalDamage -= HealOnDoingDamage;
        player = null;
    }

    private void HealOnDoingDamage(float damage)
    {
        player.health.IncreaseHealth(damage * percentHealedAttack);
    }
}
