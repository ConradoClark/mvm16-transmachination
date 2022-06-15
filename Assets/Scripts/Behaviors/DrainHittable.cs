using Licht.Unity.Physics;
using UnityEngine;

public class DrainHittable : DamageHittable
{
    public DamageSource Source;
    public Collider2D SourceCollider;
    public override bool ValidateHitSource(DamageSource hitSource)
    {
        return false;
    }

    public void Drain()
    {
        ForceInvoke(new HitEventArgs
        {
            DamageComponent = Source,
            Trigger = new CollisionTrigger
            {
                Target = SourceCollider
            }
        });
    }
}
