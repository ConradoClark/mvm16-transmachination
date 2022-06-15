using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Accessors;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;

public class BossDrain : AIAction
{
    public Vector3 Position;
    public LichtPhysicsObject BossPhysicsObject;
    public float WanderSpeed;
    public float Speed;
    public Animator Animator;

    public float TelegraphyTimeInSeconds;

    public SpriteRenderer Contour;
    public SpriteRenderer Sprite;

    private bool _wandering;

    private Color _originalContourColorize;
    private float _originalContourLightInfluence;

    private Color _originalSpriteColorize;
    private float _originalSpriteLightInfluence;

    private float _originalAnimatorSpeed;

    public Color ContourColor;
    public Color ActionColor;
    public Color DrainingColor;
    public Color DrainingContourColor;

    public bool ChangeColor;
    public bool ChangeSpeed;

    public DamageHittable DamageHitBox;
    public DashHittable DrainDashHittable;
    public DrainHittable DrainTicks;
    public Enemy Enemy;

    private bool _draining;

    private float _originalContourOpacity;
    private float _originalContourLuminance;
    private float _originalLuminance;

    public RandomPitchAudio DrainSound;
    public RandomPitchAudio BashSound;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _originalContourColorize = Contour.material.GetColor("_Colorize");
        _originalSpriteColorize = Sprite.material.GetColor("_Colorize");
        _originalContourLightInfluence = Contour.material.GetFloat("_LightInfluence");
        _originalSpriteLightInfluence = Sprite.material.GetFloat("_LightInfluence");
        _originalAnimatorSpeed = Animator.speed;

        _originalContourOpacity = Contour.material.GetFloat("_Opacity");
        _originalContourLuminance = Contour.material.GetFloat("_Luminance");
        _originalLuminance = Sprite.material.GetFloat("_Luminance");
    }

    private void OnEnable()
    {
        DrainDashHittable.OnHit += DrainDashHittable_OnHit;
    }

    private void DrainDashHittable_OnHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        _draining = false;
    }

    private void OnDisable()
    {
        DrainDashHittable.OnHit -= DrainDashHittable_OnHit;
    }


    private LerpBuilder CreateLerpBuilderForMaterial(string property, SpriteRenderer spriteRenderer)
    {
        return new LerpBuilder(f => spriteRenderer.material.SetFloat(property, f),
            () => spriteRenderer.material.GetFloat(property));
    }

    private ColorAccessor CreateColorAccessorForMaterial(string property, SpriteRenderer spriteRenderer)
    {
        return new ColorAccessor(f => spriteRenderer.material.SetColor(property, f),
            () => spriteRenderer.material.GetColor(property));
    }

    private IEnumerable<IEnumerable<Action>> StartEffect()
    {

        var lightInf = CreateLerpBuilderForMaterial("_LightInfluence", Sprite)
            .SetTarget(0.1f)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        var colorize = CreateColorAccessorForMaterial("_Colorize", Sprite)
            .ToColor(ActionColor)
            .SetTarget(1f)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        var contourLightInf = CreateLerpBuilderForMaterial("_LightInfluence", Contour)
            .SetTarget(0.1f)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        var contourColorize = CreateColorAccessorForMaterial("_Colorize", Contour)
            .ToColor(ContourColor)
            .SetTarget(1f)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        yield return lightInf.Combine(colorize)
            .Combine(contourLightInf).Combine(contourColorize);
    }

    private IEnumerable<IEnumerable<Action>> EndEffect()
    {

        var lightInf = CreateLerpBuilderForMaterial("_LightInfluence", Sprite)
            .SetTarget(_originalSpriteLightInfluence)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        var colorize = CreateColorAccessorForMaterial("_Colorize", Sprite)
            .ToColor(_originalSpriteColorize)
            .SetTarget(1f)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        var contourLightInf = CreateLerpBuilderForMaterial("_LightInfluence", Contour)
            .SetTarget(_originalContourLightInfluence)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        var contourColorize = CreateColorAccessorForMaterial("_Colorize", Contour)
            .ToColor(_originalContourColorize)
            .SetTarget(1f)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        yield return lightInf.Combine(colorize)
            .Combine(contourLightInf).Combine(contourColorize);
    }

    private IEnumerable<IEnumerable<Action>> FlickerLightInfluence()
    {
        while (_wandering)
        {
            yield return CreateLerpBuilderForMaterial("_LightInfluence", Sprite)
                .SetTarget(0.1f)
                .Over(0.05f)
                .UsingTimer(GameTimer)
                .Easing(EasingYields.EasingFunction.SineEaseOut)
                .Build();

            yield return CreateLerpBuilderForMaterial("_LightInfluence", Sprite)
                .SetTarget(_originalSpriteLightInfluence)
                .Over(0.05f)
                .UsingTimer(GameTimer)
                .Easing(EasingYields.EasingFunction.SineEaseOut)
                .Build();
        }
    }

    public override IEnumerable<IEnumerable<Action>> Run()
    {
        _wandering = true;
        DefaultMachinery.AddBasicMachine(NaturalWander());

        if (ChangeColor) yield return StartEffect().AsCoroutine();
        if (ChangeSpeed) Animator.speed = 3;

        DefaultMachinery.AddBasicMachine(FlickerLightInfluence());
        yield return TimeYields.WaitSeconds(GameTimer, TelegraphyTimeInSeconds);

        while (Vector2.Distance(BossPhysicsObject.transform.localPosition, Position) > 0.1f)
        {
            var newPos = Vector2.MoveTowards(BossPhysicsObject.transform.localPosition, Position,
                Speed * 0.001f * (float)GameTimer.UpdatedTimeInMilliseconds);
            BossPhysicsObject.ApplySpeed(newPos - (Vector2)BossPhysicsObject.transform.localPosition);
            yield return TimeYields.WaitOneFrameX;
        }

        _wandering = false;

        yield return StartDraining().AsCoroutine();

        if (ChangeColor) yield return EndEffect().AsCoroutine();

        Animator.speed = _originalAnimatorSpeed;

        yield return TimeYields.WaitOneFrameX;
    }

    private IEnumerable<IEnumerable<Action>> StartDraining()
    {
        Animator.SetBool("Draining", true);
        DrainDashHittable.enabled = true;
        DamageHitBox.enabled = false;

        _draining = true;
        var toFullLife = false;

        Contour.material.SetFloat("_Opacity", 1);
        Contour.material.SetFloat("_Luminance", 0.65f);

        var contourLightInf = CreateLerpBuilderForMaterial("_LightInfluence", Contour)
            .SetTarget(0f)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        var contourColorize = CreateColorAccessorForMaterial("_Colorize", Contour)
            .ToColor(DrainingContourColor)
            .SetTarget(1f)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .Build();

        yield return contourLightInf.Combine(contourColorize);

        while (_draining)
        {
            var lightInfOut = CreateLerpBuilderForMaterial("_LightInfluence", Sprite)
                 .SetTarget(0.15f)
                 .Over(0.1f)
                 .UsingTimer(GameTimer)
                 .BreakIf(() => !_draining)
                 .Easing(EasingYields.EasingFunction.SineEaseOut)
                 .Build();

            var colorize = CreateColorAccessorForMaterial("_Colorize", Sprite)
                .ToColor(DrainingColor)
                .SetTarget(1f)
                .Over(0.1f)
                .UsingTimer(GameTimer)
                .BreakIf(() => !_draining)
                .Easing(EasingYields.EasingFunction.SineEaseOut)
                .Build();

            var lightInfIn = CreateLerpBuilderForMaterial("_LightInfluence", Sprite)
                .SetTarget(_originalSpriteLightInfluence)
                .Over(0.1f)
                .UsingTimer(GameTimer)
                .BreakIf(() => !_draining)
                .Easing(EasingYields.EasingFunction.SineEaseOut)
                .Build();

            var colorizeBack = CreateColorAccessorForMaterial("_Colorize", Sprite)
                .ToColor(_originalSpriteColorize)
                .SetTarget(1f)
                .Over(0.1f)
                .UsingTimer(GameTimer)
                .BreakIf(() => !_draining)
                .Easing(EasingYields.EasingFunction.SineEaseOut)
                .Build();

            yield return lightInfOut.Combine(colorize);

            DrainTicks.Drain();
            DrainSound.Play();
            yield return TimeYields.WaitSeconds(GameTimer, DrainTicks.TriggerFrequencyInSeconds, breakCondition: () => !_draining);

            yield return lightInfIn.Combine(colorizeBack);

            if (Enemy.HasFullLife())
            {
                _draining = false;
                toFullLife = true;
            }
        }

        if (toFullLife) yield return EndEffect().AsCoroutine();
        else
        {
            BashSound.Play();
            yield return CreateLerpBuilderForMaterial("_Luminance", Sprite)
                .Over(0.25f)
                .SetTarget(1)
                .Easing(EasingYields.EasingFunction.CubicEaseOut)
                .UsingTimer(GameTimer)
                .Build();

            yield return CreateLerpBuilderForMaterial("_Luminance", Sprite)
                .Over(0.25f)
                .SetTarget(_originalLuminance)
                .Easing(EasingYields.EasingFunction.CubicEaseOut)
                .UsingTimer(GameTimer)
                .Build();

            yield return EndEffect().AsCoroutine();
        }

        DrainDashHittable.enabled = false;
        DamageHitBox.enabled = true;

        Contour.material.SetFloat("_Opacity", _originalContourOpacity);
        Contour.material.SetFloat("_Luminance", _originalContourLuminance);

        Animator.SetBool("Draining", false);
    }

    private IEnumerable<IEnumerable<Action>> NaturalWander()
    {
        var x1 = BossPhysicsObject.GetSpeedAccessor(Vector2.zero)
            .X
            .SetTarget(WanderSpeed * 0.01f * 0.5f)
            .Over(2f * 0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(GameTimer)
            .Build();

        var y1 = BossPhysicsObject.GetSpeedAccessor(Vector2.zero)
            .Y
            .SetTarget(WanderSpeed * 0.01f * 0.5f)
            .Over(2f * 0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();

        var x2 = BossPhysicsObject.GetSpeedAccessor(Vector2.zero)
            .X
            .SetTarget(-WanderSpeed * 0.01f * 0.5f)
            .Over(2f * 0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        var y2 = BossPhysicsObject.GetSpeedAccessor(Vector2.zero)
            .Y
            .SetTarget(-WanderSpeed * 0.01f * 0.5f)
            .Over(2f * 0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();

        while (_wandering)
        {
            yield return x1.Combine(y1);
            yield return x1.Combine(y2);
            yield return x2.Combine(y2);
            yield return x2.Combine(y1);
        }
    }
}
