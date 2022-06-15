using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Accessors;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;

public class MainMenuEffects : SceneObject<MainMenuEffects>
{
    public Transform MenuTransform;
    public SpriteRenderer BGParallax;
    public Transform Logo;

    private Vector3 _originalPosition;
    private ITime _timer;
    private SpriteRenderer[] _logoSprites;
    private TMP_Text[] _textComponents;

    private void Awake()
    {
        _timer = DefaultUITimer.GetTimer();
        _originalPosition = MenuTransform.transform.position;
        _logoSprites = Logo.GetComponents<SpriteRenderer>().Concat(Logo.GetComponentsInChildren<SpriteRenderer>())
            .ToArray();

        _textComponents = Logo.GetComponentsInChildren<TMP_Text>();
    }

    public IEnumerable<IEnumerable<Action>> FadeParallaxTo(Color color, Vector2 speed)
    {
        yield return new LerpBuilder(f=> BGParallax.material.SetFloat("_Opacity", f),
                () => BGParallax.material.GetFloat("_Opacity"))
            .SetTarget(0f)
            .Over(0.65f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_timer)
            .Build();

        BGParallax.material.SetColor("_Colorize", color);
        BGParallax.material.SetFloat("_HAutoScroll", speed.x);
        BGParallax.material.SetFloat("_VAutoScroll", speed.y);

        yield return new LerpBuilder(f => BGParallax.material.SetFloat("_Opacity", f),
                () => BGParallax.material.GetFloat("_Opacity"))
            .SetTarget(1f)
            .Over(0.65f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(_timer)
            .Build();
    }

    public IEnumerable<IEnumerable<Action>> FadeOutParallax()
    {
        yield return new LerpBuilder(f => BGParallax.material.SetFloat("_Opacity", f),
                () => BGParallax.material.GetFloat("_Opacity"))
            .SetTarget(0f)
            .Over(0.65f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_timer)
            .Build();
    }

    public IEnumerable<IEnumerable<Action>> FadeOutLogo()
    {
        var actions = _logoSprites.Select(spr => spr.GetAccessor().Color
            .A
            .SetTarget(0f)
            .Over(0.65f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_timer)
            .Build());

        var textActions = _textComponents.Select(text => new ColorAccessor(f=>text.color = f, () => text.color)
            .A
            .SetTarget(0f)
            .Over(0.65f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_timer)
            .Build());

        var combined = actions.Concat(textActions).Aggregate<IEnumerable<Action>, IEnumerable<Action>>
            (null, (current, action) => current == null ? action : current.Combine(action));

        yield return combined;
        foreach (var sprite in _logoSprites)
        {
            sprite.enabled = false;
        }
    }

    public IEnumerable<IEnumerable<Action>> FadeInLogo()
    {
        foreach (var sprite in _logoSprites)
        {
            sprite.enabled = true;
        }

        var actions = _logoSprites.Select(spr => spr.GetAccessor().Color
            .A
            .SetTarget(1f)
            .Over(0.65f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_timer)
            .Build());

        var textActions = _textComponents.Select(text => new ColorAccessor(f => text.color = f, () => text.color)
            .A
            .SetTarget(1f)
            .Over(0.65f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_timer)
            .Build());

        var combined = actions.Concat(textActions).Aggregate<IEnumerable<Action>, IEnumerable<Action>>
            (null, (current, action) => current == null ? action : current.Combine(action));

        yield return combined;
    }

    public IEnumerable<IEnumerable<Action>> HideMenu()
    {
        yield return MenuTransform.transform.GetAccessor()
            .Position
            .Y
            .Increase(1f)
            .Over(0.15f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_timer)
            .Build();

        yield return MenuTransform.transform.GetAccessor()
            .Position
            .Y
            .SetTarget(-5f)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(_timer)
            .Build();
    }
}
