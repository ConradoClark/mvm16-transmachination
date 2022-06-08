using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIAction : BaseObject
{
    public bool Selected;
    public int Order;
    public UIMenuContext MenuContext;
    public Vector2 CursorPosition;
    public abstract IEnumerable<IEnumerable<Action>> DoAction();
    public abstract void OnSelect(bool manual);
    public abstract void OnDeselect();
    public abstract void OnInit();

    protected void OnEnable()
    {
        MenuContext.AddUIAction(this, Order);
        MenuContext.OnCursorMoved += MenuContext_OnCursorMoved;
        OnInit();
    }

    protected void OnDisable()
    {
        MenuContext.OnCursorMoved -= MenuContext_OnCursorMoved;
    }

    public void SetSelected()
    {
        OnSelect(false);
    }

    private void MenuContext_OnCursorMoved(UIAction obj)
    {
        if (obj == this) OnSelect(true);
        else if (Selected) OnDeselect();
    }
}
