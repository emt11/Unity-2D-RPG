using System;
using System.Text;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    private string itemId;
    public ItemDataSO itemData;
    public ItemEffect_DataSO itemEffect;
    public int stackSize = 1;
    public ItemModifier[] modifiers { get; private set; }

    public int buyPrice { get; private set; }
    public float sellPrice { get; private set; }

    public Inventory_Item(ItemDataSO itemData)
    {
        this.itemData = itemData;
        this.itemEffect = itemData.itemEffect != null
                            ? UnityEngine.Object.Instantiate(itemData.itemEffect)
                            : null;
        modifiers = EquipmentData()?.modifiers;
        itemId = itemData.itemName + " - " + Guid.NewGuid();

        buyPrice = itemData.price;
        sellPrice = itemData.price * 0.35f;
    }

    public void AddModifiers(Entity_Stats playerStats)
    {
        foreach (var modifer in modifiers)
        {
            Stat statToModifier = playerStats.GetStatByType(modifer.statType);
            statToModifier.AddModifier(itemId, modifer.value);
        }
    }

    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var modifer in modifiers)
        {
            Stat statToModifier = playerStats.GetStatByType(modifer.statType);
            statToModifier.RemoveModifier(itemId);
        }
    }

    public void AddItemEffect(Player player) => itemEffect?.Subscribe(player);

    public void RemoveItemEffect() => itemEffect?.UnSubscribe();

    private EquipmentDataSO EquipmentData() {
        if (itemData is EquipmentDataSO equipment)
            return equipment;
        
        return null;
    }

    public bool CanAddStack() {
        return stackSize < itemData.maxStackSize;
    }

    public void IncreaseStackSize() => ++stackSize;

    public void DecreaseStackSize() => --stackSize;

    public string GetItemInfo()
    {
        if (itemData.itemType == ItemType.Material)
            return "Used for crafting.";

        if (itemData.itemType == ItemType.Consumable)
            return itemData.itemEffect.effectDescription;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(""); // \n
        foreach (var mod in modifiers)
        {
            string modType = GetStatNameByType(mod.statType);
            string modValue = mod.value.ToString() + (IsPercentageStat(mod.statType) ? "%" : "");
            sb.AppendLine("+ " + modValue + " " + modType);
        }

        if (itemEffect != null)
        {
            sb.AppendLine("");
            sb.AppendLine("Unique Effect:");
            sb.AppendLine(itemEffect.effectDescription);
        }

        return sb.ToString();
    }

    private string GetStatNameByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return "Max Health";
            case StatType.HealthRegen: return "Health Regeneration";
            case StatType.Armor: return "Armor";
            case StatType.Evasion: return "Evasion";

            case StatType.Strength: return "Strength";
            case StatType.Agility: return "Agility";
            case StatType.Intelligence: return "Intelligence";
            case StatType.Vitality: return "Vitality";

            case StatType.AttackSpeed: return "Attack Speed";
            case StatType.Damage: return "Damage";
            case StatType.CritChance: return "Critical Chance";
            case StatType.CritPower: return "Critical Power";
            case StatType.ArmorReduction: return "Armor Reduction";

            case StatType.FireDamage: return "Fire Damage";
            case StatType.IceDamage: return "Ice Damage";
            case StatType.LightningDamage: return "Lightning Damage";
            case StatType.ElementalDamage: return "Element Damage";

            case StatType.IceResistance: return "Ice Resistance";
            case StatType.FireResistance: return "Fire Resistance";
            case StatType.LightningResistance: return "Lightning Resistance";
            default: return "Unknown Stat";
        }
    }

    private bool IsPercentageStat(StatType type)
    {
        switch (type)
        {
            case StatType.CritChance:
            case StatType.CritPower:
            case StatType.ArmorReduction:
            case StatType.IceResistance:
            case StatType.FireResistance:
            case StatType.LightningResistance:
            case StatType.AttackSpeed:
            case StatType.Evasion:
                return true;
            default:
                return false;
        }
    }
}
