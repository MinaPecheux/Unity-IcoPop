using UnityEngine;

public class TargetExplodeManager : TargetManager
{
    private static float _EXPLOSION_RADIUS = 10f;

    public override void TakeHit()
    {
        if (!_canBeHit) return;
        base.TakeHit();
        foreach (TargetManager tm in MANAGERS)
        {
            if (tm == this)
                continue;
            if (Vector3.Distance(
                transform.position,
                tm.transform.position
            ) <= _EXPLOSION_RADIUS)
            {
                tm.TakeHit();
            }
        }
        _Die();
    }

}
