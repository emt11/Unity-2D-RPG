using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private Vector2 offset;

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        if (rect == null) return;

        if (!show)
        {
            rect.position = new Vector2(9999, 9999);
            return;
        }
        
        UpdatePostion(targetRect);
    }

    private void UpdatePostion(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2;
        float screenTop = Screen.height;
        float screenBottom = 0f;

        Vector2 targetPostion = targetRect.position;
        
        targetPostion.x = targetPostion.x > screenCenterX ? targetPostion.x - offset.x : targetPostion.x + offset.x;

        float verticalHalf = rect.sizeDelta.y / 2f;
        float topY = targetPostion.y + verticalHalf;
        float bottomY = targetPostion.y - verticalHalf;
        if (topY > screenTop)
            targetPostion.y = screenTop - verticalHalf - offset.y;
        else if (bottomY < screenBottom)
            targetPostion.y = screenBottom + verticalHalf + offset.y;

        rect.position = targetPostion;
    }

    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
}
