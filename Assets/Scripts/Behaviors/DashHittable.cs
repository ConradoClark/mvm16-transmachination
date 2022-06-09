using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DashHittable : Hittable<DamageSource>
{
    public DamageSource.DamageSourceType HitType;
    public override bool ValidateHitSource(DamageSource hitSource)
    {
        return hitSource.Enabled && hitSource.SourceType != HitType && hitSource.Damage.DamageType == DamageType.Dash;
    }
}
