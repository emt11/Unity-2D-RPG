using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Grant Skill Point", fileName = "Item Effect Data - Grant Skill Point")]
public class ItemEffect_GrandSkillPoints : ItemEffect_DataSO
{
    [SerializeField] private int skillPointToAdd;

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        UI ui = FindAnyObjectByType<UI>();
        Debug.Log("Grant Skill Points" + skillPointToAdd);
        ui.skillTreeUI.AddSkillPoints(skillPointToAdd);
    }
}
