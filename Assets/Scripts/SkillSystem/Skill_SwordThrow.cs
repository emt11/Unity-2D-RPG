using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{
    private SkillObject_Sword currentSword;
    private float currentThrowPower;

    [Header("Regular Sword Upgrade")]
    [SerializeField] private GameObject regularSwordPrefab;
    [Range(0, 10)]
    [SerializeField] private float regularThrowPower = 5f;

    [Header("Peirce Sword Upgrade")]
    [SerializeField] private GameObject peirceSwordPrefab;
    public int amountToPierce = 2;
    [Range(0, 10)]
    [SerializeField] private float peirceThrowPower = 5f;

    [Header("Spin Sword Upgrade")]
    [SerializeField] private GameObject spinSwordPrefab;
    public float maxDistance = 7f;
    public float maxDuration = 3f;
    public int attackPerSecond = 6;
    [Range(0, 10)]
    [SerializeField] private float spinThrowPower = 5f;

    [Header("Bounce Sword Upgrade")]
    [SerializeField] private GameObject bounceSwordPrefab;
    public float bounceSpeed = 15f;
    public int bounceCounts = 5;
    [Range(0, 10)]
    [SerializeField] private float bounceThrowPower = 5f;

    [Header("Trajectory Prediction")]
    [SerializeField] private GameObject predictionDot;
    [SerializeField] private int numberOfDots = 20;
    [SerializeField] private float spaceBetweenDots = 0.05f;
    private float swordGravity;
    private Transform[] dots;
    private Vector2 confirmedDirection;

    protected override void Awake()
    {
        base.Awake();
        dots = GenerateDots();
    }

    public override bool CanUseSkill()
    {
        UpdateCurrentThrowPower();
        if (currentSword != null) // 抛出剑之后必须收回才能继续使用
        {
            currentSword.GetSwordBackToPlayer();
            return false;
        }

        return base.CanUseSkill();
    }

    public void ThrowSword()
    {
        GameObject swordPrefab = GetSwordPrefab();
        if (swordPrefab == null) return;
        swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale;
        GameObject newSword = Instantiate(swordPrefab, dots[1].position, Quaternion.identity);
        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetupSword(this, GetThrowPower());
        SetSkillOnCoolDown();
    }

    private GameObject GetSwordPrefab()
    {
        if (Unlocked(SkillUpgradeType.SwordThrow))
            return regularSwordPrefab;
        else if (Unlocked(SkillUpgradeType.SwordThrow_Pierce))
            return peirceSwordPrefab;
        else if (Unlocked(SkillUpgradeType.SwordThrow_Spin))
            return spinSwordPrefab;
        else if (Unlocked(SkillUpgradeType.SwordThrow_Bounce))
            return bounceSwordPrefab;
        else 
            return null;
    }

    private void UpdateCurrentThrowPower() {
        switch (upgradeType)
        {
            case SkillUpgradeType.SwordThrow: 
                currentThrowPower = regularThrowPower;
                break;
            case SkillUpgradeType.SwordThrow_Pierce: 
                currentThrowPower = peirceThrowPower;
                break;
            case SkillUpgradeType.SwordThrow_Spin: 
                currentThrowPower = spinThrowPower;
                break;
            case SkillUpgradeType.SwordThrow_Bounce: 
                currentThrowPower = bounceThrowPower;
                break;
            default:
                currentThrowPower = regularThrowPower;
                break;
        }
    }

    private Vector2 GetThrowPower() => confirmedDirection * (currentThrowPower * 10);

    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; ++i)
        {
            dots[i].position = GetTrajectoryPoint(direction, i * spaceBetweenDots);
        }
    }

    // 给定方向，预测轨迹点位
    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = currentThrowPower * 10;

        // This gives us the initial velocity  the starting speed and direction of the throw.
        Vector2 initialVelocity = direction * scaledThrowPower;

        // Gravity pulls the sword down over time. The longer it's in the air, the more it drops.
        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (t * t);

        // We calculate how far the sword will travel after time 't',
        // by combining the initial throw direction with the gravity pull.
        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect;

        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }

    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction;

    public void EnableDots(bool enable)
    {
        foreach (Transform t in dots)
            t.gameObject.SetActive(enable);
    }

    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[numberOfDots];
        for (int i = 0; i < numberOfDots; ++i)
        {
            newDots[i] = Instantiate(predictionDot, transform.position, Quaternion.identity, transform).transform;
            newDots[i].gameObject.SetActive(false);
        }

        return newDots;
    }
}
