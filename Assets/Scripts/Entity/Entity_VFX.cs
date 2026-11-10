using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private Entity entity;
    protected SpriteRenderer sr;

    [Header("Image Echo VFX")]
    [Range(0.01f, 0.2f)]
    [SerializeField] private float imageEchoInterval = 0.5f;
    [SerializeField] private GameObject imageEchoPrefab;
    private Coroutine imageEchoCo;

    [Header("On Taking Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = 0.15f;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    [Header("On Doing Damage VFX")]
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;
    [SerializeField] private Color hitVfxColor = Color.white;

    [Header("Element Colors")]
    [SerializeField] private Color chillVfxColor = Color.cyan;
    [SerializeField] private Color burnVfxColor = Color.red;
    [SerializeField] private Color electrifyVfxColor = Color.yellow;
    private Color originalHitVfxColor;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        entity = GetComponent<Entity>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
    }

    public void StopAllVfxs()
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice) 
            StartCoroutine(PlayStatusVfxCo(duration, chillVfxColor));
        else if (element == ElementType.Fire)
            StartCoroutine(PlayStatusVfxCo(duration, burnVfxColor));
        else if (element == ElementType.Lightning)
            StartCoroutine(PlayStatusVfxCo(duration, electrifyVfxColor));
    }

    // 元素效果：闪烁
    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.25f;
        float timeHasPassed = 0f;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * 0.8f;

        bool toggle = false;
        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);

            timeHasPassed += tickInterval;
        }

        sr.color = Color.white;
    }

    public void CreateOnHitVFX(Transform target, bool isCrit, ElementType element) {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        // vfx.GetComponentInChildren<SpriteRenderer>().color = GetElementColor(element); 让 basic attack 不附着颜色
        if (entity.facingDir == -1) // 保证 critVfx Flip
            vfx.transform.Rotate(0, 180, 0);
    }

    public Color GetElementColor(ElementType element)
    {
        if (element == ElementType.Ice)
            return chillVfxColor;
        else if (element == ElementType.Fire)
            return burnVfxColor;
        else if (element == ElementType.Lightning)
            return electrifyVfxColor;
        else
            return originalHitVfxColor;
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
    }

    public void DoImageEchoEffect(float duration)
    {
        StopImageEchoEffect();
        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration));
    }

    public void StopImageEchoEffect()
    {
        if (imageEchoCo != null)
            StopCoroutine(imageEchoCo);
    }

    private IEnumerator ImageEchoEffectCo(float duration)
    {
        float timeTracker = 0f;
        while (timeTracker < duration)
        {
            CreateImageEcho();
            yield return new WaitForSeconds(imageEchoInterval);
            timeTracker += imageEchoInterval;
        }
    }

    private void CreateImageEcho()
    {
        Vector3 position = entity.anim.transform.position;
        float scale = entity.anim.transform.localScale.x;

        GameObject imageEcho = Instantiate(imageEchoPrefab, position, transform.rotation);
        imageEcho.transform.localScale = new Vector3(scale, scale, scale);
        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }
}
