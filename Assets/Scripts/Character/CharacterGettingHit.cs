using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterGettingHit : BaseObject
{
    public DamageHittable DamageHittable;
    public SpriteRenderer[] CharacterSprites;
    public SpriteRenderer ContourSprite;
    public float RecoilStrength;

    public float BlinkFrequencyInMs;

    private Player _player;
    private bool _blink;
    private float _originalLightInfluence;
    private EffectsManager _effectsManager;

    private void OnEnable()
    {
        DamageHittable.OnHit += DamageHittable_OnHit;
        _player = Player.Instance();
        _originalLightInfluence = ContourSprite.material.GetFloat("_LightInfluence");
        _effectsManager = EffectsManager.Instance();
    }

    private void DamageHittable_OnHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        DefaultMachinery.AddBasicMachine(HandleHit(obj));
    }

    private IEnumerable<IEnumerable<Action>> HandleHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        _blink = true;

        _player.Stats.TakeDamage(obj.DamageComponent.Damage.DamageAmount);

        if (_effectsManager.HitNumberPool.TryGetFromPool(out var effect))
        {
            effect.SetHitValue(obj.DamageComponent.Damage.DamageAmount);

            effect.transform.position = obj.Trigger.Target.transform.position + new Vector3(0, 0.15f) +
                                        (Vector3)Random.insideUnitCircle * 0.25f;
        }

        ContourSprite.material.SetColor("_Colorize", Color.yellow);
        ContourSprite.material.SetFloat("_LightInfluence", 0);
        BlockMovement();
        var actions = Blink(BlinkFrequencyInMs).AsCoroutine().Combine(
            UnblockActions(_player.GettingHitDurationInSeconds).AsCoroutine()
        ).Combine(Recoil(obj).AsCoroutine());

        DefaultMachinery.AddBasicMachine(actions);
        yield return TimeYields.WaitSeconds(GameTimer, _player.InvincibilityDurationInSeconds);
        ContourSprite.material.SetColor("_Colorize", new Color(0,0,0,1));
        ContourSprite.material.SetFloat("_LightInfluence", _originalLightInfluence);
        _blink = false;
    }

    private void BlockMovement()
    {
        _player.JumpController.BlockMovement(this);
        _player.MoveController.BlockMovement(this);
        _player.BlasterController.BlockMovement(this);
        _player.DashController.BlockMovement(this);
    }

    private IEnumerable<IEnumerable<Action>> Recoil(Hittable<DamageSource>.HitEventArgs obj)
    {
        if (obj.DamageComponent.Damage.DamageType == DamageType.Contact)
        {
            yield return _player.PhysicsObject.GetSpeedAccessor()
                .X
                .SetTarget(RecoilStrength * Mathf.Sign(_player.MoveController.LatestDirection * -1) * 0.001f)
                .Over(_player.GettingHitDurationInSeconds)
                .UsingTimer(GameTimer)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .Build();
        }

        else yield return _player.PhysicsObject.GetSpeedAccessor()
            .X
            .SetTarget(RecoilStrength * Mathf.Sign(obj.Trigger.Actor.LatestSpeed.x) * 0.001f)
            .Over(_player.GettingHitDurationInSeconds)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> UnblockActions(float waitTimeInSeconds)
    {
        yield return TimeYields.WaitSeconds(GameTimer, waitTimeInSeconds);
        _player.JumpController.UnblockMovement(this);
        _player.MoveController.UnblockMovement(this);
        _player.BlasterController.UnblockMovement(this);
        _player.DashController.UnblockMovement(this);
    }

    private IEnumerable<IEnumerable<Action>> Blink(float frequency)
    {
        while (_blink)
        {
            HideSprites();
            yield return TimeYields.WaitMilliseconds(GameTimer, frequency, breakCondition: () => !_blink);
            ShowSprites();
            yield return TimeYields.WaitMilliseconds(GameTimer, frequency, breakCondition: () => !_blink);
        }
    }

    private void OnDisable()
    {
        DamageHittable.OnHit -= DamageHittable_OnHit;
    }

    private void HideSprites()
    {
        foreach (var sprite in CharacterSprites)
        {
            sprite.enabled = false;
        }
    }

    private void ShowSprites()
    {
        foreach (var sprite in CharacterSprites)
        {
            sprite.enabled = true;
        }
    }
}
