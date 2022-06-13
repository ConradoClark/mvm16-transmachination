using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using UnityEngine;

public class DashCrystal : RoomObject
{
    public ScriptableTriggerWatcher TriggerWatcher;
    public DashHittable HitTrigger;
    public SpriteRenderer Sprite;
    public Light Light;

    private float _initialLightIntensity;
    private float _initialLightInfluence;

    private bool _hooked;

    public override bool PerformReset()
    {
        SetState();
        return true;
    }

    public override void Initialize()
    {
        _initialLightIntensity = Light.intensity;
        _initialLightInfluence = Sprite.material.GetFloat("_LightInfluence");
    }

    public override bool Activate()
    {
        if (_hooked) return false;

        HitTrigger.OnHit += HitTrigger_OnHit;
        TriggerWatcher.OnTriggerChanged += TriggerWatcher_OnTriggerChanged;
        _hooked = true;
        SetState();
        return true;
    }

    private void TriggerWatcher_OnTriggerChanged(bool obj)
    {
        if (obj)
        {
            DefaultMachinery.AddBasicMachine(TurnOn());
        }
        else
        {
            DefaultMachinery.AddBasicMachine(TurnOff());
        }
    }

    private void HitTrigger_OnHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        TriggerWatcher.Trigger.Triggered = !TriggerWatcher.Trigger.Triggered;
    }

    private IEnumerable<IEnumerable<Action>> TurnOn()
    {
        Light.enabled = true;
        Light.intensity = 0;
        var lightInfluence = new LerpBuilder(f => Sprite.material.SetFloat("_LightInfluence", f),
                () => Sprite.material.GetFloat("_LightInfluence"))
            .SetTarget(_initialLightInfluence + 0.55f)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        var lightFade = new LerpBuilder(f=> Light.intensity = f, () => Light.intensity)
            .SetTarget(_initialLightIntensity)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        yield return lightInfluence.Combine(lightFade);
    }

    private IEnumerable<IEnumerable<Action>> TurnOff()
    {
        var lightInfluence = new LerpBuilder(f => Sprite.material.SetFloat("_LightInfluence", f),
                () => Sprite.material.GetFloat("_LightInfluence"))
            .SetTarget(_initialLightInfluence)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        var lightFade = new LerpBuilder(f => Light.intensity = f, () => Light.intensity)
            .SetTarget(0f)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        yield return lightInfluence.Combine(lightFade);
        Light.enabled = false;
    }

    public override bool Deactivate()
    {
        HitTrigger.OnHit -= HitTrigger_OnHit;
        TriggerWatcher.OnTriggerChanged -= TriggerWatcher_OnTriggerChanged;
        _hooked = false;
        return true;
    }

    private void SetState()
    {
        if (TriggerWatcher.Trigger.Triggered)
        {
            Light.enabled = true;
            Light.intensity = _initialLightIntensity;
            Sprite.material.SetFloat("_LightInfluence", _initialLightInfluence+0.15f);
        }
        else
        {
            Light.enabled = false;
            Light.intensity = _initialLightIntensity;
            Sprite.material.SetFloat("_LightInfluence", _initialLightInfluence);
        }
    }
}
