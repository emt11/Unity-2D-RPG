using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();
    private float finalValue;

    private bool needToBeCalculated = true;

    public float GetValue()
    {
        if (needToBeCalculated)
        {
            finalValue = GetFinalValue();
            needToBeCalculated = false;
        }

        return finalValue;
    }

    public void AddModifier(string source, float value)
    {
        modifiers.Add(new StatModifier(source, value));
        needToBeCalculated = true;
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);
        needToBeCalculated = true;
    }

    private float GetFinalValue()
    {
        float finalValue = baseValue;
        modifiers.ForEach(modifier => finalValue += modifier.value);
        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;
}

[Serializable]
public class StatModifier
{
    [SerializeField] public string source;
    [SerializeField] public float value;

    public StatModifier(string source, float value)
    {
        this.source = source;
        this.value = value;
    }
}
