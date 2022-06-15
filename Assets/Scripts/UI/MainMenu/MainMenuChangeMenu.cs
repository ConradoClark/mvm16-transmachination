using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using UnityEngine;

public class MainMenuChangeMenu: MainMenuUIButton
{
    public Color BGParallaxColor;
    public Vector2 BGParallaxScroll;

    public Transform TargetMenu;
    private Vector2 _originalMenuPosition;
    private ITime _timer;

    protected override void UnityAwake()
    {
        _originalMenuPosition = TargetMenu.transform.position;
        _timer = DefaultUITimer.GetTimer();
    }

    public override IEnumerable<IEnumerable<Action>> OnAction()
    {
        yield return 
            MainMenuEffects.HideMenu().AsCoroutine().Combine(
            MainMenuEffects.FadeOutLogo().AsCoroutine()).Combine(
                MainMenuEffects.FadeParallaxTo(BGParallaxColor, BGParallaxScroll).AsCoroutine());

        MenuContext.gameObject.SetActive(false);
        TargetMenu.gameObject.SetActive(true);

        TargetMenu.transform.position = new Vector3(TargetMenu.transform.position.x, 6);
        yield return TargetMenu.transform.GetAccessor()
            .Position
            .Y
            .SetTarget(_originalMenuPosition.y)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.BounceEaseOut)
            .UsingTimer(_timer)
            .Build();
    }
}
