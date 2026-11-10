using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDatabaseSO audioDB;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Space]
    private Transform player;

    private AudioClip lastMusicPlayed;
    private string currentBgmGroupName;
    private Coroutine currentBgmCo;
    [SerializeField] private bool bgmSourcePlay;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!bgmSource.isPlaying && bgmSourcePlay)
        {
            if (string.IsNullOrEmpty(currentBgmGroupName)) return;
            NextBGM(currentBgmGroupName);
        }

        if (bgmSource.isPlaying && !bgmSourcePlay)
            StopBGM();
    }

    public void StartBGM(string musicGroup)
    {
        bgmSourcePlay = true;
        if (musicGroup == currentBgmGroupName)
            return;

        NextBGM(musicGroup);
    }

    public void NextBGM(string musicGroup)
    {
        bgmSourcePlay = true;
        currentBgmGroupName = musicGroup;

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);

        currentBgmCo = StartCoroutine(SwtichMusciCo(musicGroup));
    }

    public void StopBGM()
    {
        bgmSourcePlay = false;
        StartCoroutine(FadeVolumeCo(bgmSource, 0f, 1f));

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);
    }

    private IEnumerator SwtichMusciCo(string musicGroup)
    {
        AudioClipData data = audioDB.Get(musicGroup);
        if (data == null || data.clips.Count == 0) yield break;

        AudioClip nextMusic = data.GetRandomClip();
        if (data.clips.Count > 1)
        {
            while (nextMusic == lastMusicPlayed)
                nextMusic = data.GetRandomClip();
        }

        // 停止当前播放 Music
        if (bgmSource.isPlaying)
            yield return FadeVolumeCo(bgmSource, 0f, 1f);

        // 播放 musicGroup 的新 Music
        lastMusicPlayed = nextMusic;
        bgmSource.clip = nextMusic;
        bgmSource.volume = 0;
        bgmSource.Play();
        StartCoroutine(FadeVolumeCo(bgmSource, data.maxVolume, 1f));
    }

    private IEnumerator FadeVolumeCo(AudioSource source, float targetVolume, float duration)
    {
        float time = 0f;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void PlaySFX(string soundName, AudioSource sfxSource, float minDistanceToHearSound = 15f)
    {
        if (player == null)
            player = Player.instance.transform;

        AudioClipData data = audioDB.Get(soundName);
        if (data == null)
        {
            Debug.Log("Attempt to play sound - " + soundName);
            return;
        }

        AudioClip clip = data.GetRandomClip();
        if (clip == null)
            return;

        float maxVolume = data.maxVolume;
        float distance = Vector3.Distance(player.position, sfxSource.transform.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceToHearSound));
        sfxSource.volume = Mathf.Lerp(0f, maxVolume, t * t);

        sfxSource.pitch = Random.Range(0.90f, 1.1f);
        sfxSource.PlayOneShot(clip); 
    }

    public void PlayerGlobalSFX(string soundName)
    {
        AudioClipData data = audioDB.Get(soundName);
        if (data == null) return;

        AudioClip clip = data.GetRandomClip();
        if (clip == null) return;

        sfxSource.volume = data.maxVolume;
        sfxSource.PlayOneShot(clip);
    }
}
