using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{
    private UI ui;
    private Player_QuestManager questManager;
    private DialogueNpcData npcData;

    [SerializeField] private Image speakerPortrait;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI[] dialogueChoices;

    [Space]
    [SerializeField] private float textSpeed = 0.1f;
    
    private DialogueLineSO currentLine;
    private string fullTextToShow;
    private bool waitingForConfirm;
    private bool canInteract;
    
    private DialogueLineSO[] currentChoices;
    private DialogueLineSO selectedChoice;
    private int selectedChoiceIndex;

    private Coroutine typingCo;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        questManager = Player.instance.questManager;
    }

    public void PlayDialogueLine(DialogueLineSO line)
    {
        // 清理上一轮对话状态
        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            typingCo = null;
        }
        selectedChoice = null;
        selectedChoiceIndex = 0;
        canInteract = false;
        waitingForConfirm = false;
        HideAllChoices();

        currentLine = line;
        currentChoices = line.choiceLines;
        speakerPortrait.sprite = line.speaker.speakerPortrait;
        speakerName.text = line.speaker.speakerName;
        fullTextToShow = line.actionType == DialogueActionType.NONE || line.actionType == DialogueActionType.PlayerMakeChoice
                            ? line.GetRandomLine()
                            : line.actionLine;

        typingCo = StartCoroutine(TypeTextCo(fullTextToShow));
        StartCoroutine(EnableInteractionCo());
    }

    public void DialogueInteraction()
    {
        if (!canInteract) return;

        if (typingCo != null)
        {
            CompleteTyping();

            if (currentLine.actionType == DialogueActionType.PlayerMakeChoice)
                HandleNextAction();
            else
                waitingForConfirm = true;
            
            return;
        }

        if (waitingForConfirm || selectedChoice != null)
            HandleNextAction();
    }

    private void HandleNextAction()
    {
        switch (currentLine.actionType) {
            case DialogueActionType.OpenShop:
                ui.SwitchToInGameUI(); // 隐藏当前显示 UI 
                ui.ToggleMerchantUI(true);
                break;
            
            case DialogueActionType.PlayerMakeChoice:
                if (selectedChoice == null)
                    ShowChoices();
                else
                {
                    PlayDialogueLine(selectedChoice);
                    selectedChoice = null;
                }
                break;
            
            case DialogueActionType.OpenQuest:
                ui.SwitchToInGameUI();
                ui.OpenQuestUI(npcData.quests);
                break;

            case DialogueActionType.OpenCraft:
                ui.SwitchToInGameUI();
                ui.ToggleStorageUI(true);
                break;

            case DialogueActionType.GetQuestReward:
                ui.SwitchToInGameUI();
                questManager.GetQuestRewardFrom(npcData.rewardType);
                break;

            case DialogueActionType.CloseDialogue:
            case DialogueActionType.NONE:
            default:
                ui.SwitchToInGameUI();
                break;
        }
    }

    private void CompleteTyping()
    {
        if (typingCo != null)
        {
            StopCoroutine(typingCo);
            dialogueText.text = fullTextToShow;
            typingCo = null;
        }
    }

    private void ShowChoices()
    {
        for (int i = 0; i < dialogueChoices.Length; ++i)
        {
            if (i < currentChoices.Length)
            {
                string choiceText = currentChoices[i].playerChoiceAnswer;
                dialogueChoices[i].gameObject.SetActive(true);
                dialogueChoices[i].text = selectedChoiceIndex == i 
                    ? $"<color=yellow> {i + 1}) {choiceText}" 
                    : $"{i + 1}) {choiceText}";

                if (currentChoices[i].actionType == DialogueActionType.GetQuestReward && 
                        !questManager.HasClaimableQuest(npcData.rewardType))
                    dialogueChoices[i].gameObject.SetActive(false);
            }
            else
            {
                dialogueChoices[i].gameObject.SetActive(false);
            }
        }

        selectedChoice = currentChoices[selectedChoiceIndex];
    }

    private void HideAllChoices()
    {
        foreach (var choice in dialogueChoices)
            choice.gameObject.SetActive(false);
    }

    public void NavigateChoice(int direction)
    {
        if (currentChoices == null || currentChoices.Length <= 1)
            return;
        
        selectedChoiceIndex += direction;
        selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, currentChoices.Length - 1);
        ShowChoices();
    }

    private IEnumerator TypeTextCo(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        
        if (currentLine.actionType == DialogueActionType.PlayerMakeChoice)
        {
            yield return new WaitForSeconds(0.2f);

            HandleNextAction();
        }
        else
            waitingForConfirm = true;

        typingCo = null;
    }

    private IEnumerator EnableInteractionCo()
    {
        yield return null;
        canInteract = true;
    }

    public void SetupNpcData(DialogueNpcData npcData) => this.npcData = npcData;
}
