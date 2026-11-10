using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Buff Effect", fileName = "Item Effect Data - Buff")]
public class ItemEffect_Buff : ItemEffect_DataSO
{
    private string source = Guid.NewGuid().ToString();
    [SerializeField] private float duration;
    [SerializeField] private BuffEffectData[] buffsToApply;

    public override bool CanBeUsed(Player player)
    {
        if (player.stats.CanApplyBuffOf(source))
        {
            this.player = player;
            return true;
        }
        else
        {
            Debug.Log("Same buff effect cannot be applied twice!");
            return false;
        }
    }

    public override void ExecuteEffect()
    {
        player.stats.ApplyBuff(source, buffsToApply, duration);
        player = null;
    }
}
