using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using UnityEngine;

public class MainMenuBackButton : MainMenuUIButton
{
    public Vector2 BGParallaxScroll;
    public Vector2 MainMenuPosition;
    public UIMenuContext MainMenu;
    private ITime _timer;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _timer = DefaultUITimer.GetTimer();
    }

    private IEnumerable<IEnumerable<Action>> HideCurrentMenu()
    {
        yield return MenuContext.transform.GetAccessor()
            .Position
            .Y
            .Increase(1f)
            .Over(0.15f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_timer)
            .Build();

        yield return MenuContext.transform.GetAccessor()
            .Position
            .Y
            .SetTarget(-6f)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(_timer)
            .Build();
    }

    public override IEnumerable<IEnumerable<Action>> OnAction()
    {
        yield return
            HideCurrentMenu().AsCoroutine().Combine(
                MainMenuEffects.FadeParallaxTo(new Color(0, 0, 0, 0), BGParallaxScroll).AsCoroutine());

        MenuContext.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(true);

        MainMenu.transform.position = new Vector3(MainMenu.transform.position.x, 6);

        DefaultMachinery.AddBasicMachine(MainMenuEffects.FadeInLogo());

        yield return MainMenu.transform.GetAccessor()
            .Position
            .Y
            .SetTarget(MainMenuPosition.y)
            .Over(0.8f)
            .Easing(EasingYields.EasingFunction.BounceEaseOut)
            .UsingTimer(_timer)
            .Build();
    }
}
