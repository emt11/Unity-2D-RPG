using UnityEngine;

public class Object_Waypoint : MonoBehaviour
{
    [SerializeField] private string tranferToScene; // 传送场景名称
    [Space]
    public RespawnType waypointType; // 当前类型
    [SerializeField] private RespawnType connectedWaypoint; // 传送类型， Exit -> tranferToScene.Enter, Enter -> tranferToScene.Exit
    [SerializeField] private Transform respwanPoint; // 传送点位
    [SerializeField] private bool canBeTriggered = true; // 是否可以触发传送

    private void OnValidate()
    {
        gameObject.name = "Object_Waypoint - " + waypointType.ToString() + " - " + tranferToScene;

        if (waypointType == RespawnType.Enter)
            connectedWaypoint = RespawnType.Exit;
        else if (waypointType == RespawnType.Exit)
            connectedWaypoint = RespawnType.Enter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeTriggered) return;

        SaveManager.instance.SaveGame();
        GameManager.instance.ChangeScene(tranferToScene, connectedWaypoint); // 跳转到其他场景
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
    }

    public void SetCanBeTriggered(bool canBeTriggered) => this.canBeTriggered = canBeTriggered;

    public RespawnType GetRespawnType() => waypointType;

    public Vector3 GetPositionAndSetTriggerFalse()
    {
        canBeTriggered = false;
        return respwanPoint != null ? respwanPoint.position : transform.position;
    }
}
