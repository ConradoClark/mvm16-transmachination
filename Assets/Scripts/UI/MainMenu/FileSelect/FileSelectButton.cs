using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FileSelectButton: MainMenuUIButton
{
    private ITime _timer;

    protected override void UnityAwake()
    {
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
                MainMenuEffects.FadeOutParallax().AsCoroutine());

        MenuContext.gameObject.SetActive(false);

        // Load file
        // Change to correct scene

        DefaultMachinery.FinalizeWith(() =>
        {
            SceneManager.LoadScene("Scenes/UndergroundFacility", LoadSceneMode.Single);
        });
    }
}
