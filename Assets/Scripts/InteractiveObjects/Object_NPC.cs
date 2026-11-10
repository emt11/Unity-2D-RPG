using UnityEngine;

public class Object_NPC : MonoBehaviour, IInteractable
{
    protected UI ui;
    protected Transform player;
    protected Player_QuestManager questManager;

    [Header("Quest Info")]
    [SerializeField] private string npcTargetQuestId;
    [SerializeField] protected RewardType npcType;

    [SerializeField] private Transform npc;
    [SerializeField] private GameObject interactTooltip;
    private bool facingRight = true;

    [Header("Yoffset Details")]
    [SerializeField] private float floatSpeed = 8f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    protected virtual void Awake()
    {
        ui = FindAnyObjectByType<UI>();
        interactTooltip.SetActive(false);
        startPosition = interactTooltip.transform.localPosition;
    }

    protected virtual void Start()
    {
        questManager = Player.instance.questManager;
    }

    protected virtual void Update()
    {
        HandleToolTipFloat();
        HandleNpcFlip();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.transform;
        interactTooltip.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        interactTooltip.SetActive(false);
    }

    private void HandleNpcFlip()
    {
        if (player == null || npc == null)
            return;

        if (npc.position.x > player.position.x && facingRight)
        {
            npc.Rotate(0, 180, 0);
            facingRight = false;
        } 
        else if (npc.position.x < player.position.x && !facingRight)
        {
            npc.Rotate(0, 180, 0);
            facingRight = true;
        }
    }

    private void HandleToolTipFloat()
    {
        if (interactTooltip.activeSelf)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactTooltip.transform.localPosition = startPosition + new Vector3(0, yOffset, 0);
        }
    }

    public virtual void Interact()
    {
        questManager.AddProcess(npcTargetQuestId, 1);
    }
}
