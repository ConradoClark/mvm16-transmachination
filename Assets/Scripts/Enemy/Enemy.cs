using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Impl.Time;
using Licht.Interfaces.Time;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HitPoints;
    public SpriteRenderer SpriteRenderer;
    public Collider2D Collider;

    private BasicMachinery<object> _defaultMachinery;
    private LichtPhysics _physics;
    private ITime _gameTimer;
    private float _defaultLuminance;
    private float _defaultOpacity;
    private bool _flashing;
    private bool _resetFlash;

    private void Awake()
    {
        _defaultMachinery = DefaultMachinery.GetDefaultMachinery();
        _physics = this.GetLichtPhysics();
        _gameTimer = DefaultGameTimer.GetTimer();
        _defaultLuminance = SpriteRenderer.material.GetFloat("_Luminance");
        _defaultOpacity = SpriteRenderer.material.GetFloat("_Opacity");
    }

    private void OnEnable()
    {
        _defaultMachinery.AddBasicMachine(CheckCollision());
    }

    private IEnumerable<IEnumerable<Action>> Flash()
    {
        if (_flashing)
        {
            _resetFlash = true;
            do
            {
                yield return TimeYields.WaitOneFrameX;
            } while (_flashing);
        }

        _resetFlash = false;
        _flashing = true;
        SpriteRenderer.material.SetFloat("_Luminance", _defaultLuminance);

        SpriteRenderer.material.SetFloat("_Opacity", 1);

        yield return new LerpBuilder(f => SpriteRenderer.material.SetFloat("_Luminance", f), () =>
                SpriteRenderer.material.GetFloat("_Luminance"))
            .SetTarget(1)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .Over(0.1f)
            .BreakIf(()=> _resetFlash)
            .UsingTimer(_gameTimer)
            .Build();

        yield return new LerpBuilder(f => SpriteRenderer.material.SetFloat("_Luminance", f), () =>
                SpriteRenderer.material.GetFloat("_Luminance"))
            .SetTarget(_defaultLuminance)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .Over(0.1f)
            .BreakIf(() => _resetFlash)
            .UsingTimer(_gameTimer)
            .Build();

        SpriteRenderer.material.SetFloat("_Opacity", _defaultOpacity);
        _flashing = false;
    }

    private IEnumerable<IEnumerable<Action>> CheckCollision()
    {
        while (isActiveAndEnabled)
        {
            if (_physics.CheckCollision(Collider, out var trigger) && trigger.Actor.TryGetCustomObject<WeaponDamage>(out var weaponDamage))
            {
                _defaultMachinery.AddBasicMachine(Flash());
                yield return TimeYields.WaitMilliseconds(_gameTimer, 50);
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
