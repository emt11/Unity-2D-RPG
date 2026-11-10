using UnityEngine;

public class Object_Merchant : Object_NPC
{
    [Header("Quest & Dialogue")]
    [SerializeField] private QuestDataSO[] quests;
    [SerializeField] private DialogueLineSO firstDialogueLine;

    private Inventory_Player inventory;
    private Inventory_Merchant merchant;

    protected override void Awake()
    {
        base.Awake();
        merchant = GetComponent<Inventory_Merchant>();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Z))
            merchant.FillShopList();
    }

    public override void Interact()
    {
        if (inventory == null)
            return;
            
        base.Interact();
        
        ui.merchantUI.SetupMerchantUI(inventory, merchant);
        ui.OpenDialogueUI(firstDialogueLine, new DialogueNpcData(npcType, quests));
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = collision.GetComponent<Inventory_Player>();
        merchant.SetInventory(inventory);
    }

    // 已经不会使用了，因为打开 UI 默认禁止玩家移动
    protected override void OnTriggerExit2D(Collider2D collider)
    {
        if (!Application.isPlaying || ui == null)
            return;

        base.OnTriggerExit2D(collider);
        ui.HideAllToolTips();
        ui.ToggleMerchantUI(false);
    }
}
