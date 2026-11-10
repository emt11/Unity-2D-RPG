using UnityEngine;

public class AudioRangeController : MonoBehaviour
{
    [SerializeField] private float minDistanceToHearSound = 15f;
    [SerializeField] private bool drawGizmos = false;

    private AudioSource audioSource;
    private Transform player;
    private float maxVolume;

    private void Start()
    {
        player = Player.instance.transform;
        audioSource = GetComponent<AudioSource>();
        maxVolume = audioSource.volume;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(audioSource.transform.position, player.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceToHearSound));
        float targetVolume = Mathf.Lerp(0, maxVolume, t * t);
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime);
    }

    public void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minDistanceToHearSound);
    }
}
