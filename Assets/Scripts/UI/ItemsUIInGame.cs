using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

public class ItemsUIInGame : BaseUIObject
{
    public ScriptableTriggerWatcher TriggerWatcher;
    public SpriteRenderer SpriteRenderer;

    private void OnEnable()
    {
        SpriteRenderer.enabled = TriggerWatcher.Trigger.Triggered;
        TriggerWatcher.OnTriggerChanged += TriggerWatcher_OnTriggerChanged;
    }

    private void TriggerWatcher_OnTriggerChanged(bool obj)
    {
        SpriteRenderer.enabled = obj;
    }

    private void OnDisable()
    {
        TriggerWatcher.OnTriggerChanged -= TriggerWatcher_OnTriggerChanged;
    }
}
