using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using UnityEngine;

public class MiniDashCrystal : RoomObject
{
    public DashHittable Hitbox;
    public SpriteRenderer Sprite;

    private float _originalLightInfluence;
    private const string LightStrength = "_LightStrength";

    public override bool PerformReset()
    {
        return true;
    }

    public override void Initialize()
    {
        _originalLightInfluence = Sprite.material.GetFloat(LightStrength);
    }

    public override bool Deactivate()
    {
        Hitbox.OnHit -= Hitbox_OnHit;
        return base.Deactivate();
    }

    public override bool Activate()
    {
        Hitbox.OnHit += Hitbox_OnHit;
        return true;
    }

    private void Hitbox_OnHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        DefaultMachinery.AddBasicMachine(Flash());
    }

    private IEnumerable<IEnumerable<Action>> Flash()
    {
        yield return new LerpBuilder(f => Sprite.material.SetFloat(LightStrength, f),
                () => Sprite.material.GetFloat(LightStrength))
            .SetTarget(10f)
            .Over(0.20f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        yield return new LerpBuilder(f => Sprite.material.SetFloat(LightStrength, f),
                () => Sprite.material.GetFloat(LightStrength))
            .SetTarget(_originalLightInfluence)
            .Over(0.15f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(GameTimer)
            .Build();
    }
}
