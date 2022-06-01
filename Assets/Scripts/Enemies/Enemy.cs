using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Interfaces.Update;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IResettable
{
    public int HitPoints;
    public SpriteRenderer SpriteRenderer;
    public DamageHittable[] HitDetectors;

    private BasicMachinery<object> _defaultMachinery;
    private ITime _gameTimer;
    private float _defaultLuminance;
    private float _defaultOpacity;
    private bool _flashing;
    private bool _resetFlash;
    private EffectsManager _effectsManager;
    private int _currentHitPoints;

    private void Awake()
    {
        _defaultMachinery = DefaultMachinery.GetDefaultMachinery();
        _gameTimer = DefaultGameTimer.GetTimer();
        _defaultLuminance = SpriteRenderer.material.GetFloat("_Luminance");
        _defaultOpacity = SpriteRenderer.material.GetFloat("_Opacity");
        _effectsManager = EffectsManager.Instance();
        _currentHitPoints = HitPoints;
    }

    private void OnEnable()
    {
        foreach (var hitDetector in HitDetectors)
        {
            hitDetector.OnHit += HitDetector_OnHit;
        }
    }

    private void OnDisable()
    {
        foreach (var hitDetector in HitDetectors)
        {
            hitDetector.OnHit -= HitDetector_OnHit;
        }
    }

    private void HitDetector_OnHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        if (_effectsManager.HitNumberPool.TryGetFromPool(out var effect))
        {
            // calculate damage elsewhere
            var damage = obj.DamageComponent.Damage.DamageAmount + Random.Range(-1, 2);
            effect.SetHitValue(damage);
            _currentHitPoints -= damage;

            effect.transform.position = obj.Trigger.Target.transform.position + new Vector3(0, 0.15f) +
                                        (Vector3)Random.insideUnitCircle * 0.25f;
        }

        if (_currentHitPoints <= 0) gameObject.SetActive(false);

        _defaultMachinery.AddBasicMachine(Flash());
    }

    // write this in a better way: not all enemies will flash the same way
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
            .BreakIf(() => _resetFlash)
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

    public bool PerformReset()
    {
        _currentHitPoints = HitPoints;
        _flashing = false;
        return true;
    }
}
