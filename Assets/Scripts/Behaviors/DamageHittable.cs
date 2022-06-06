public class DamageHittable : Hittable<DamageSource>
{
    public DamageSource.DamageSourceType HitType;
    public override bool ValidateHitSource(DamageSource hitSource)
    {
        return hitSource.Enabled && hitSource.SourceType != HitType;
    }
}
