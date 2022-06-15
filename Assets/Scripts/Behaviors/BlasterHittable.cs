public class BlasterHittable : Hittable<DamageSource>
{
    public DamageSource.DamageSourceType HitType;
    public override bool ValidateHitSource(DamageSource hitSource)
    {
        return hitSource.SourceType != HitType && hitSource.Damage.DamageType == DamageType.Blaster;
    }
}
