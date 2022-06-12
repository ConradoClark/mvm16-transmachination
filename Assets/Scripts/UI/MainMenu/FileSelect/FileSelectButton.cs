using System;
using System.Collections.Generic;
using System.IO;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FileSelectButton : MainMenuUIButton
{
    public TMP_Text NewFileCaption;
    public Transform SaveDetails;

    public SavePoint CurrentSavePoint;
    public FileSelect FileSelect;
    private ITime _timer;

    protected override void UnityAwake()
    {
        _timer = DefaultUITimer.GetTimer();
    }

    public override void OnInit()
    {
        base.OnInit();
        if (FileSelect.SaveFile.Created)
        {
            ShowFile();
        }
        else
        {
            ShowEmptyFile();
        }
    }

    private void ShowFile()
    {
        SaveDetails.gameObject.SetActive(true);
        NewFileCaption.enabled = false;
    }

    private void ShowEmptyFile()
    {
        SaveDetails.gameObject.SetActive(false);
        NewFileCaption.enabled = true;
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

        CurrentSavePoint.LoadFromSavePoint(FileSelect.SaveFile);

        DefaultMachinery.FinalizeWith(() =>
        {
            SceneManager.LoadScene("Scenes/UndergroundFacility", LoadSceneMode.Single);
        });
    }
}
