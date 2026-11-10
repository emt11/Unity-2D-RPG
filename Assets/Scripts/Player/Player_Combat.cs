using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter attack details")]
    [SerializeField] private float counterDuration;
    [SerializeField] private LayerMask whatIsCounterable;

    public override void Awake()
    {
        base.Awake();
        whatIsTarget = LayerMask.GetMask("Enemy", "Chest");
    }

    public bool CounterAttackPerformered()
    {
        bool hasCounteredSomebody = false;
        foreach (var target in GetDetectedColliders(whatIsCounterable))
        {
            ICounterable counterable = target.GetComponent<ICounterable>();
            if (counterable == null) continue;
            if (counterable.CanBeCountered)
            {
                counterable.HandleCounter();
                hasCounteredSomebody = true;
            }
        }

        return hasCounteredSomebody;
    }

    public float GetCounterDuration() => counterDuration;
}
