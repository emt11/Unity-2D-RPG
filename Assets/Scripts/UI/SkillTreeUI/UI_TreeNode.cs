using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectionHandler connectionHandler;

    [Header("Unlock Details")]
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;
    public bool isUnlocked;
    public bool isLocked;

    [Header("Skill Details")]
    [SerializeField] public Skill_DataSO skillData;
    [SerializeField] public string skillName;
    [SerializeField] public int skillCost;
    [SerializeField] private Image skillIcon;
    [SerializeField] private string lockedColorHex = "#9F9F9F";
    private Color lastColor;

    private void Start()
    {
        UpdateIconColor(isUnlocked ? Color.white : GetColorByHex(lockedColorHex));
        UnlockDefaultSkill();
    }

    public void UnlockDefaultSkill()
    {
        GetNeededComponents();

        if (skillData.unlockedByDefault)
            Unlock();
    }

    private void GetNeededComponents()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>(true);
        connectionHandler = GetComponent<UI_TreeConnectionHandler>();
    }

    private void Unlock()
    {
        if (isUnlocked)
            return;
        
        isUnlocked = true;
        skillTree.RemoveSkillPoints(skillCost);
        UpdateIconColor(Color.white);
        LockConflictNodes();
        connectionHandler.ConnectionImageUnlocked(true);

        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData);
    }

    public void UnlockWithSaveData()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNodes();
        connectionHandler.ConnectionImageUnlocked(true);
    }

    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)
            return false;
        
        if (!skillTree.EnoughSkillPoints(skillCost))
            return false;

        foreach (var neededNode in neededNodes)
            if (!neededNode.isUnlocked)
                return false;

        foreach (var conflictNode in conflictNodes)
            if (conflictNode.isUnlocked)
                return false;

        return true;
    }

    private void LockConflictNodes()
    {
        foreach (var node in conflictNodes)
            node.LockChildNode();
    }

    private void LockChildNode()
    {
        isLocked = true;
        foreach (var node in connectionHandler.GetChildNodes())
            node.LockChildNode();
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null) 
            return;
        
        lastColor = skillIcon.color;
        skillIcon.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, skillData, this);
        if (!isUnlocked || !isLocked)
            return;
        ToggleNodeHighlight(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
            Unlock();
        else if (isLocked)
            ui.skillToolTip.LockedSkillEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, null);
        ui.skillToolTip.StopLockedSKillEffect();
        if (!isUnlocked || !isLocked)
            return;
        
        ToggleNodeHighlight(true);
    }

    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * 0.9f; highlightColor.a = 1;
        Color colorToApply = highlight ? highlightColor : lastColor;
        UpdateIconColor(colorToApply);
    }

    private void OnDisable()
    {
        UpdateIconColor(GetColorByHex(lockedColorHex));
        if (isUnlocked)
            UpdateIconColor(Color.white);
    }

    public void Refund()
    {
        if (!isUnlocked || skillData.unlockedByDefault) // 只有解锁才 refund 
            return;
        
        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        skillTree.AddSkillPoints(skillCost);
        connectionHandler.ConnectionImageUnlocked(false);

        // TODO skill manage and reset skill
    }

    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);
        return color;
    }

    // 当 Inspector 的内容改变时调用
    private void OnValidate()
    {
        if (skillData == null) return;
        skillName = skillData.displayName;
        skillCost = skillData.cost;
        skillIcon.sprite = skillData.icon;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;
    }
}
