using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI ui;
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCooldown;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    [Space]
    [SerializeField] private string metConditionHex;
    [SerializeField] private string notMetConditionHex;
    [SerializeField] private string importantInfoHex;
    [SerializeField] private Color exampleColor;
    [SerializeField] private string lockedSkillText = "You've taken a diffient path - this skill is now locked!";

    private Coroutine textEffectCo;

    protected override void Awake()
    {
        base.Awake();
        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>(true);
    }

    public void LockedSkillEffect()
    {
        StopLockedSKillEffect();
        textEffectCo = StartCoroutine(TextBlinkEffectCo(skillRequirements, 0.15f, 3));
    }

    public void StopLockedSKillEffect()
    {
        if (textEffectCo != null) 
            StopAllCoroutines();
    }
    
    private IEnumerator TextBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)
    {
        for (int i = 0; i < blinkCount; ++i)
        {
            text.text = GetColoredText(notMetConditionHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);
            text.text = GetColoredText(importantInfoHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    public void ShowToolTip(bool show, RectTransform targetRect, Skill_DataSO skillData, UI_TreeNode node)
    {
        base.ShowToolTip(show, targetRect);

        skillName.text = skillData.displayName;
        skillDescription.text = skillData.description;
        skillCooldown.text = "Cooldown: " + skillData.upgradeData.cooldown + " s.";
        if (node == null)
        {
            skillRequirements.text = "";
            return;
        }

        string skillLockedText = GetColoredText(importantInfoHex, lockedSkillText);
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillCost, node.neededNodes, node.conflictNodes);
        skillRequirements.text = requirements;
    }

    public string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Requirements:");
        
        // 技能点是否足够
        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notMetConditionHex;
        string costText = $"- {skillCost} skill point(s)";
        string finalCostText = GetColoredText(costColor, costText);
        sb.AppendLine(finalCostText);

        // 前置节点是否解锁
        foreach (var node in neededNodes)
        {
            if (node == null) continue;
            string nodeColor = node.isUnlocked ? metConditionHex : notMetConditionHex;
            string nodeText = $"- {node.skillName}";
            string finalNodeText = GetColoredText(nodeColor, nodeText);
            sb.AppendLine(finalNodeText); 
        }

        // 显示冲突节点
        if (conflictNodes.Length <= 0)
            return sb.ToString();

        sb.AppendLine(); // \n
        sb.AppendLine(GetColoredText(importantInfoHex, "Locks out: "));
        foreach (var node in conflictNodes)
        {
            if (node == null) continue;
            sb.AppendLine(GetColoredText(importantInfoHex, $"- {node.skillName}"));
        }

        return sb.ToString();
    }
}
