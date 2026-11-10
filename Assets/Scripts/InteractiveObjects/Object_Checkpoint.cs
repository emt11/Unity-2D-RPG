using UnityEngine;

public class Object_Checkpoint : MonoBehaviour, ISaveable
{
    [SerializeField] private string checkpointId;
    [SerializeField] private Transform respawnPoint;
    private AudioSource audioSource;
    private Animator anim;

    public bool isActive { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(checkpointId))
            checkpointId = System.Guid.NewGuid().ToString();
#endif
    }

    public string GetCheckpointId() => checkpointId;

    public Vector3 GetPosition() => respawnPoint != null ? respawnPoint.position : transform.position;

    public void ActivateCheckpoint(bool activate)
    {
        isActive = activate;
        anim.SetBool("isActive", activate);

        if (isActive && !audioSource.isPlaying)
            audioSource.Play();
        
        if (!isActive)
            audioSource.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActivateCheckpoint(true);
    }

    public void SaveData(ref GameData gameData)
    {
        if (!isActive) return;

        if (!gameData.unlockedCheckpoints.ContainsKey(checkpointId))
            gameData.unlockedCheckpoints.Add(checkpointId, true);
    }

    public void LoadData(GameData gameData)
    {
        bool active = gameData.unlockedCheckpoints.TryGetValue(checkpointId, out active);
        ActivateCheckpoint(active);
    }
}
