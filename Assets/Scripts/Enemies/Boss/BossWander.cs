using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Accessors;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;

public class BossWander : AIAction
{
    public List<Vector3> Positions;
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

    public bool ChangeColor;
    public bool ChangeSpeed;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _originalContourColorize = Contour.material.GetColor("_Colorize");
        _originalSpriteColorize = Sprite.material.GetColor("_Colorize");
        _originalContourLightInfluence = Contour.material.GetFloat("_LightInfluence");
        _originalSpriteLightInfluence = Sprite.material.GetFloat("_LightInfluence");
        _originalAnimatorSpeed = Animator.speed;
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

        foreach (var pos in Positions)
        {
            while (Vector2.Distance(BossPhysicsObject.transform.localPosition, pos) > 0.1f)
            {
                var newPos = Vector2.MoveTowards(BossPhysicsObject.transform.localPosition, pos,
                    Speed * 0.001f * (float)GameTimer.UpdatedTimeInMilliseconds);
                BossPhysicsObject.ApplySpeed(newPos - (Vector2)BossPhysicsObject.transform.localPosition);
                yield return TimeYields.WaitOneFrameX;
            }
        }

        if (ChangeColor) yield return EndEffect().AsCoroutine();

        Animator.speed = _originalAnimatorSpeed;
        _wandering = false;

        yield return TimeYields.WaitOneFrameX;
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
