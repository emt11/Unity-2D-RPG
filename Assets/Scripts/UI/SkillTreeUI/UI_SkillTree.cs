using System.Linq;
using TMPro;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour, ISaveable
{
    [SerializeField] private int skillPoints;
    [SerializeField] private TextMeshProUGUI skillPointsText;
    [SerializeField] private UI_TreeConnectionHandler[] parentNodes;
    private UI_TreeNode[] allTreeNodes;
    public Player_SkillManager skillManager { get; private set; }

    private void Start()
    {
        UpdateAllConnections();
        UpdateSkillPointsUI();
    }

    private void UpdateSkillPointsUI()
    {
        skillPointsText.text = skillPoints.ToString();
    }

    public void UnlockDefaultSkills()
    {
        skillManager = FindAnyObjectByType<Player_SkillManager>();
        allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);
        foreach (UI_TreeNode node in allTreeNodes)
            node.UnlockDefaultSkill();
    }

    [ContextMenu("Reset All Skills")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();
        foreach (var node in skillNodes) {
            node.Refund();
        }
    }

    [ContextMenu("Update All Connections")]
    private void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
            node.UpdateAllConnections();
    }

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;

    public void RemoveSkillPoints(int cost)
    {
        skillPoints -= cost;
        UpdateSkillPointsUI();
    } 

    public void AddSkillPoints(int cost) {
        skillPoints += cost;
        UpdateSkillPointsUI();
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.skillTreeUI.Clear();
        gameData.skillUpgrades.Clear();

        gameData.skillPoints = skillPoints;

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillName;
            gameData.skillTreeUI[skillName] = node.isUnlocked;
        }

        foreach (var skill in skillManager.allSkills)
        {
            gameData.skillUpgrades[skill.GetSkillType()] = skill.GetUpgraderType();
        }
    }

    public void LoadData(GameData gameData)
    {
        skillPoints = gameData.skillPoints;
        
        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillName;
            if (gameData.skillTreeUI.TryGetValue(skillName, out bool unlocked) && unlocked)
                node.UnlockWithSaveData();
        }

        foreach (var skill in skillManager.allSkills)
        {
            if (gameData.skillUpgrades.TryGetValue(skill.GetSkillType(), out SkillUpgradeType upgradeType)) {
                var upgradeNode = allTreeNodes.FirstOrDefault(node => node.skillData.upgradeData.upgradeType == upgradeType);
                if (upgradeNode != null)
                    skill.SetSkillUpgrade(upgradeNode.skillData);
            }
        }
    }
}
