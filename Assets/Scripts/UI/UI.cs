using System;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    [SerializeField] private GameObject[] uiElements;
    public bool alternativeInput { get; private set; }
    private PlayerInputSet input;

    #region UI Components
    public UI_SkillToolTip skillToolTip { get; private set; }
    public UI_ItemToolTip itemToolTip { get; private set; }
    public UI_StatToolTip statToolTip { get; private set; }
    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Invectory inventoryUI { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Merchant merchantUI { get; private set; }
    public UI_InGame inGameUI { get; private set; }
    public UI_Options optionsUI { get; private set; }
    public UI_DeathScreen deathScreenUI { get; private set; }
    public UI_FadeScreen fadeScreenUI { get; private set; }
    public UI_Quest questUI { get; private set; }
    public UI_ActiveQuest activeQuestUI { get; private set; }
    public UI_Dialogue dialogueUI { get; private set; }
    #endregion

    private bool skillTreeUIEnabled;
    private bool inventoryUIEnabled;

    private void Awake()
    {
        instance = this;

        skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        itemToolTip = GetComponentInChildren<UI_ItemToolTip>();
        statToolTip = GetComponentInChildren<UI_StatToolTip>();
        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
        inventoryUI = GetComponentInChildren<UI_Invectory>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        merchantUI = GetComponentInChildren<UI_Merchant>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        optionsUI = GetComponentInChildren<UI_Options>(true);
        deathScreenUI = GetComponentInChildren<UI_DeathScreen>(true);
        fadeScreenUI = GetComponentInChildren<UI_FadeScreen>(true);
        questUI = GetComponentInChildren<UI_Quest>(true);
        activeQuestUI = GetComponentInChildren<UI_ActiveQuest>(true);
        dialogueUI = GetComponentInChildren<UI_Dialogue>(true);

        skillTreeUIEnabled = skillTreeUI.gameObject.activeSelf;
        inventoryUIEnabled = inventoryUI.gameObject.activeSelf;
    }

    private void Start()
    {
        skillTreeUI.UnlockDefaultSkills();
    }

    public void SetupControlUI(PlayerInputSet input)
    {
        this.input = input;

        input.UI.InventoryUI.performed += ctx => ToggleInventoryUI();
        input.UI.SkillTreeUI.performed += ctx => ToggleSkillTreeUI();

        input.UI.AlternativeInput.performed += ctx => alternativeInput = true;
        input.UI.AlternativeInput.canceled += ctx => alternativeInput = false;

        input.UI.OptionsUI.performed += ctx =>
        {
            foreach (var ui in uiElements)
            {
                if (ui.activeSelf)
                {
                    Time.timeScale = 1;
                    SwitchToInGameUI();
                    return;
                }
            }
            
            Time.timeScale = 0;
            OpenOptionsUI();
        };

        input.UI.DialogueInteraction.performed += ctx =>
        {
            if (dialogueUI.gameObject.activeSelf)
                dialogueUI.DialogueInteraction();
        };

        input.UI.DialogueNavigation.performed += ctx =>
        {
            int direction = Mathf.RoundToInt(ctx.ReadValue<float>());
            if (dialogueUI.gameObject.activeSelf)
                dialogueUI.NavigateChoice(direction);
        };
    }

    public void OpenQuestUI(QuestDataSO[] quests)
    {
        HideAllToolTips();
        StopPlayerControls(false);
        questUI.gameObject.SetActive(true);
        
        questUI.SetupQuestUI(quests);
    }

    public void OpenDialogueUI(DialogueLineSO firstLine, DialogueNpcData npcData)
    {
        StopPlayerControls(true);
        HideAllToolTips();

        dialogueUI.gameObject.SetActive(true);
        dialogueUI.SetupNpcData(npcData);
        dialogueUI.PlayDialogueLine(firstLine);
    }

    public void OpenDeathScreenUI()
    {
        SwitchTo(deathScreenUI.gameObject);
        input.Disable();
    }

    public void OpenOptionsUI()
    {
        HideAllToolTips();
        SwitchTo(optionsUI.gameObject);
        StopPlayerControls(true);
    }

    public void SwitchToInGameUI()
    {
        HideAllToolTips();
        SwitchTo(inGameUI.gameObject);
        StopPlayerControls(false);

        skillTreeUIEnabled = false;
        inventoryUIEnabled = false;
    }

    private void SwitchTo(GameObject uiToSwitchOn)
    {
        foreach (var ui in uiElements)
            ui.gameObject.SetActive(false);

        uiToSwitchOn.SetActive(true);
    }

    public void StopPlayerControls(bool stopControls)
    {
        if (stopControls)
            input.Player.Disable();
        else
            input.Player.Enable();
    }

    public void HideAllToolTips()
    {
        if (itemToolTip != null)
            itemToolTip.ShowToolTip(false, null);

        if (skillToolTip != null)
            skillToolTip.ShowToolTip(false, null);

        if (statToolTip != null)
            statToolTip.ShowToolTip(false, null);
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeUIEnabled = !skillTreeUIEnabled;
        skillTreeUI.gameObject.SetActive(skillTreeUIEnabled);
        skillToolTip.ShowToolTip(false, null);

        StopPlayerControls(skillTreeUIEnabled);
    }

    public void ToggleInventoryUI()
    {
        bool shouldClose = inventoryUI.gameObject.activeSelf || activeQuestUI.gameObject.activeSelf;
        inventoryUI.gameObject.SetActive(false);
        activeQuestUI.gameObject.SetActive(false);
        if (!shouldClose)
            inventoryUI.gameObject.SetActive(true);

        inventoryUIEnabled = !shouldClose;
        itemToolTip.ShowToolTip(false, null);
        statToolTip.ShowToolTip(false, null);

        StopPlayerControls(inventoryUIEnabled);
    }

    public void ToggleStorageUI(bool storageUIEnabled)
    {
        storageUI.gameObject.SetActive(storageUIEnabled);
        StopPlayerControls(storageUIEnabled);

        if (!storageUIEnabled)
        {
            craftUI.gameObject.SetActive(false);
            HideAllToolTips();
        }
    }

    public void ToggleMerchantUI(bool merchantUIEnabled)
    {
        merchantUI.gameObject.SetActive(merchantUIEnabled);
        StopPlayerControls(merchantUIEnabled);

        if (!merchantUIEnabled)
            HideAllToolTips();
    }
}
