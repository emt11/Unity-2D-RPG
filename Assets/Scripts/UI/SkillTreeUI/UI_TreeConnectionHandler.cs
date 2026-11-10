using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UI_TreeConnectionDetails
{
    public UI_TreeConnectionHandler childNode;
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length;
    [Range(-50f, 50f)] public float rotation;
}

public class UI_TreeConnectionHandler : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectionDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnection[] connections;

    private Image connectionImage;
    private Color originalColor;

    private void Awake()
    {
        if (connectionImage != null)
            originalColor = connectionImage.color;
    }

    public UI_TreeNode[] GetChildNodes()
    {
        List<UI_TreeNode> childrenNodeList = new List<UI_TreeNode>();
        foreach (var node in connectionDetails)
            if (node.childNode != null)
                childrenNodeList.Add(node.childNode.GetComponent<UI_TreeNode>());
        return childrenNodeList.ToArray();        
    }

    private void OnValidate()
    {
        if (connectionDetails.Length <= 0)
            return;
            
        if (connectionDetails.Length != connections.Length)
        {
            Debug.Log("Amount of details should be same as amount of connections" + gameObject.name);
            return;
        }
        UpdateConnections();
    }

    private void UpdateConnections()
    {
        for (int i = 0; i < connectionDetails.Length; ++i)
        {
            var detail = connectionDetails[i];
            var connection = connections[i];

            if (detail == null || connection == null) continue;
            // 设置连接属性：长度和角度
            connection.DirectConnection(detail.direction, detail.length, detail.rotation);

            if (detail.childNode == null) continue;
            // 设置 details 子节点的 锚点位置为 Connection 中的 childNodeConnectionPoint 位置
            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            detail.childNode?.SetPosition(targetPosition);
            // 设置 details 连接边 的颜色
            Image connectionImage = connection.GetConnectionImage();
            detail.childNode?.SetConnectionImage(connectionImage); // 孩子节点保存当前父节点连接线
            detail.childNode.transform.SetAsLastSibling(); // 保证孩子节点在父节点之后渲染
        }
    }

    public void UpdateAllConnections()
    {
        UpdateConnections();
        foreach (var node in connectionDetails)
        {
            if (node.childNode == null) continue;
            node.childNode.UpdateConnections();
        }
    }

    public void ConnectionImageUnlocked(bool unlocked)
    {
        if (connectionImage == null) return;
        connectionImage.color = unlocked ? Color.white : originalColor; // 实际上设置的是父节点与当前节点的连接线颜色！
    }

    public void SetConnectionImage(Image image) => connectionImage = image;

    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
