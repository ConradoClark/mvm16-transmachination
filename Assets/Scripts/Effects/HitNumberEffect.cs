using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using Licht.Unity.Pooling;
using TMPro;
using UnityEngine;

public class HitNumberEffect : EffectPoolable
{
    public TMP_Text TextComponent;
    private BasicMachinery<object> _defaultMachinery;
    private ITime _gameTimer;

    private void Awake()
    {
        _defaultMachinery = DefaultMachinery.GetDefaultMachinery();
        _gameTimer = DefaultGameTimer.GetTimer();
    }

    public override void OnActivation()
    {
        _defaultMachinery.AddBasicMachine(Animate());
    }

    public void SetHitValue(int value)
    {
        TextComponent.text = value.ToString();
    }

    public IEnumerable<IEnumerable<Action>> Animate()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        var size = transform.GetAccessor()
            .UniformScale()
            .SetTarget(1.25f)
            .Over(0.25f)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .UsingTimer(_gameTimer)
            .Build();

        var sizeDecrease = transform.GetAccessor()
            .UniformScale()
            .SetTarget(1f)
            .Over(0.15f)
            .Easing(EasingYields.EasingFunction.SineEaseIn)
            .UsingTimer(_gameTimer)
            .Build();

        var goUp = transform.GetAccessor()
            .Position.Y
            .Increase(0.5f)
            .Over(0.5f)
            .UsingTimer(_gameTimer)
            .Build();

        var shrink = transform.GetAccessor()
            .UniformScale()
            .SetTarget(0.1f)
            .Over(0.15f)
            .Easing(EasingYields.EasingFunction.SineEaseOut)
            .UsingTimer(_gameTimer)
            .Build();

        yield return goUp.Combine(size.Then(sizeDecrease)).Then(shrink);

        IsEffectOver = true;
    }

    public override bool IsEffectOver { get; protected set; }
}
