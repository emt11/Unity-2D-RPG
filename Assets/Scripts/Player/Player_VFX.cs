using UnityEngine;

public class Player_VFX : Entity_VFX
{
    public void CreateEffectOf(GameObject vfx, Transform target)
    {
        Instantiate(vfx, target.position, Quaternion.identity);
    }
}
