using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;

public abstract class MainMenuUIButton : UIAction
{
    public SpriteRenderer BGSpriteRenderer;
    public Color SelectedColor;
    public Color UnselectedColor;
    public AudioSource ClickSound;

    protected MainMenuEffects MainMenuEffects;

    public abstract IEnumerable<IEnumerable<Action>> OnAction();

    public override IEnumerable<IEnumerable<Action>> DoAction()
    {
        if (ClickSound != null) ClickSound.Play();
        OnDeselect();
        yield return OnAction().AsCoroutine();
    }

    public override void OnSelect(bool manual)
    {
        Selected = true;
        AdjustColor();
    }

    public override void OnDeselect()
    {
        Selected = false;
        AdjustColor();
    }

    public override void OnInit()
    {
        AdjustColor();
        MainMenuEffects = MainMenuEffects.Instance();
    }

    private void AdjustColor()
    {
        BGSpriteRenderer.color = Selected ? SelectedColor : UnselectedColor;
    }
}
