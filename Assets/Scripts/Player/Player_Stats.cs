using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : Entity_Stats
{
    private List<string> activeBuffs = new List<string>();

    private Inventory_Player inventory;

    protected override void Awake()
    {
        base.Awake();
        inventory = GetComponent<Inventory_Player>();
    }

    public bool CanApplyBuffOf(string source)
    {
        return !activeBuffs.Contains(source);
    }

    public void ApplyBuff(string source, BuffEffectData[] buffsToApply, float duration)
    {
        StartCoroutine(BuffCo(source, buffsToApply, duration));
    }

    private IEnumerator BuffCo(string source, BuffEffectData[] buffsToApply, float duration)
    {
        activeBuffs.Add(source);

        foreach (var buff in buffsToApply) {
            GetStatByType(buff.type).AddModifier(source, buff.value);
        }

        yield return new WaitForSeconds(duration);

        foreach (var buff in buffsToApply)
        {
            GetStatByType(buff.type).RemoveModifier(source);
        }

        activeBuffs.Remove(source);
        inventory.TriggerUpdateUI();
    }
}
