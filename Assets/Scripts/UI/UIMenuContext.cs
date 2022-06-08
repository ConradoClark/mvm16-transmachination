using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIMenuContext : BaseObject
{
    public enum UIDirection
    {
        Horizontal,
        Vertical
    }

    public UIDirection Direction;
    public UICursor Cursor;
    public ScriptableInputAction UIHorizontal;
    public ScriptableInputAction UIVertical;
    public ScriptableInputAction UIAccept;
    public ScriptableInputAction UICancel;
    public UIAction CancelAction;

    private SortedList<int, UIAction> _actions;
    private PlayerInput _input;
    private KeyValuePair<int, UIAction> _currentAction;

    public event Action<UIAction> OnActionClicked;
    public event Action<UIAction> OnCursorMoved;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _input = PlayerInput.GetPlayerByIndex(0);
        _actions ??= new SortedList<int, UIAction>();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleMenu());
    }

    private void OnDisable()
    {
        _actions.Clear();
    }

    public void AddUIAction(UIAction action, int order)
    {
        _actions ??= new SortedList<int, UIAction>();
        _actions.Add(order, action);
    }

    private IEnumerable<IEnumerable<Action>> HandleMenu()
    {
        if (_actions.Count == 0) yield break;

        var uiDirection = _input.actions[Direction == UIDirection.Horizontal ? UIHorizontal.ActionName : UIVertical.ActionName];
        var uiAccept = _input.actions[UIAccept.ActionName];
        var uiCancel = _input.actions[UICancel.ActionName];
        _currentAction = _actions.First();
        _currentAction.Value.SetSelected();
        Cursor.transform.localPosition = _currentAction.Value.CursorPosition;

        while (isActiveAndEnabled)
        {
            if (uiDirection.WasPerformedThisFrame())
            {
                var dir = Mathf.Sign(uiDirection.ReadValue<float>());
                var num = (_actions.IndexOfKey(_currentAction.Key) + (dir > 0 ? -1 : +1));
                _currentAction = _actions.Skip((num < 0 ? _actions.Count-1 : num) % _actions.Count)
                        .FirstOrDefault();
                Cursor.transform.localPosition = _currentAction.Value.CursorPosition;

                OnCursorMoved?.Invoke(_currentAction.Value);
            }

            if (uiAccept.WasPerformedThisFrame())
            {
                yield return _currentAction.Value.DoAction().AsCoroutine();
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
