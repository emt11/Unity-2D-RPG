using UnityEngine;

public class Enemy_ReaperAnimationTrigger : Enemy_AnimationTrigger
{
    private Enemy_Reaper reaper;

    public override void Awake()
    {
        base.Awake();
        reaper = GetComponentInParent<Enemy_Reaper>();
    }

    private void TeleportTrigger(AnimationEvent evt)
    {
reaper.SetTeleportTrigger(true);
    }
}
