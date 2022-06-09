﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using UnityEngine;

public class ScriptableTriggerWatcher : BaseObject
{
    public ScriptableTrigger Trigger;

    private bool _initialValue;
    protected override void UnityAwake()
    {
        base.UnityAwake();
        _initialValue = Trigger.Triggered;
    }

    private void OnEnable()
    {
        _initialValue = Trigger.Triggered;
        DefaultMachinery.AddBasicMachine(WatchTrigger());
    }

    public event Action<bool> OnTriggerChanged;

    private IEnumerable<IEnumerable<Action>> WatchTrigger()
    {
        while (isActiveAndEnabled)
        {
            if (Trigger.Triggered != _initialValue)
            {
                OnTriggerChanged?.Invoke(Trigger.Triggered);
                _initialValue = Trigger.Triggered;
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
