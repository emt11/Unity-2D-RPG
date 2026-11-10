using UnityEngine;

public class Object_Blacksmith : Object_NPC
{
    private Animator anim;
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    [Header("Dialogue")]
    [SerializeField] private DialogueLineSO firstDialogueLine;

    protected override void Awake()
    {
        base.Awake();
        storage = GetComponent<Inventory_Storage>();
        
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("blacksmith", true);
    }

    public override void Interact()
    {
        if (inventory == null)
            return;
        
        base.Interact();

        ui.storageUI.SetupStorageUI(storage);
        ui.craftUI.SetupCraftUI(storage);

        ui.OpenDialogueUI(firstDialogueLine, null);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<Inventory_Player>();
        storage.SetPlayerInventory(inventory);
    }

    // 已经不会使用了，因为打开 UI 默认禁止玩家移动
    protected override void OnTriggerExit2D(Collider2D collider)
    {
        base.OnTriggerExit2D(collider);
        ui.HideAllToolTips();
        ui.ToggleStorageUI(false);
    }
}