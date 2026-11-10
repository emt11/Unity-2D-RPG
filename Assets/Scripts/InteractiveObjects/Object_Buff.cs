using System.Collections;
using UnityEngine;

public class Object_Buff : MonoBehaviour
{
    private Player_Stats playerStats;

    [Header("Buff Details")]
    [SerializeField] private BuffEffectData[] buffs;
    [SerializeField] private string buffName = "GoodGameBuff";

    [Header("Buffer Details")]
    [SerializeField] private float buffDuration;

    [Header("Yoffset Details")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    public void Awake()
    {
        startPosition = transform.position;
    }

    public void Update()
    {
        // 上下浮动
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerStats = collision.GetComponent<Player_Stats>();
        if (playerStats.CanApplyBuffOf(buffName))
        {
            playerStats.ApplyBuff(buffName, buffs, buffDuration);
            Destroy(gameObject);
        }
    }
}
