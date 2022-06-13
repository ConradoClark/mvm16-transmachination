using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseAction : UIAction
{
    public SpriteRenderer BGSpriteRenderer;
    public Color SelectedColor;
    public Color UnselectedColor;

    public override IEnumerable<IEnumerable<Action>> DoAction()
    {
        yield break;
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
    }

    private void AdjustColor()
    {
        if (BGSpriteRenderer == null) return;
        BGSpriteRenderer.color = Selected ? SelectedColor : UnselectedColor;
    }
}
