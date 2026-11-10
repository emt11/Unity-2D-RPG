using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected UI ui;
    protected RectTransform rect;
    public Inventory_Item itemInSlot { get; private set; }
    protected Inventory_Player inventory;

    [Header("UI Slot Setup")]
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected TextMeshProUGUI itemStackSize;
        
    protected virtual void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        inventory = FindAnyObjectByType<Inventory_Player>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        bool alternativeInput = Input.GetKey(KeyCode.LeftControl);
        if (alternativeInput)
        {
            inventory.RemoveItem(itemInSlot);
        }
        else
        {
            ItemType itemType = itemInSlot.itemData.itemType;
            if (itemType == ItemType.Material)
                return;
            else if (itemType == ItemType.Consumable)
                inventory.TryUseItem(itemInSlot);
            else
                inventory.TryEquipItem(itemInSlot);
        }
        
        if (itemInSlot == null)
            ui.itemToolTip.ShowToolTip(false, null);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null) return;
        
        ui.itemToolTip.ShowToolTip(true, rect, itemInSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemToolTip.ShowToolTip(false, null);
    }

    public void UpdateItemSlot(Inventory_Item item)
    {
        itemInSlot = item;
        if (itemInSlot == null)
        {
            itemStackSize.text = "";
            itemIcon.color = Color.clear;
            return;
        }
        Color color = Color.white; color.a = 0.9f;
        itemIcon.color = color;
        itemIcon.sprite = itemInSlot.itemData.itemIcon;
        itemStackSize.text = itemInSlot.stackSize.ToString();
    }
}
