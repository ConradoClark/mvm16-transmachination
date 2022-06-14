using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEditorInternal;
using UnityEngine;

public class BossDoor : RoomObject
{
    public Animator Animator;
    public bool Open;
    public RoomExit TargetRoomExit;
    public Collider2D Collider;

    private Player _player;
    private bool _temporarilyOpen;
    public float DistanceBeforeClosing;

    public ScriptableTriggerWatcher TriggerWatcher;

    public override void PerformDestroy()
    {
        TriggerWatcher.OnTriggerChanged -= TriggerWatcherOnOnTriggerChanged;
    }

    public override bool PerformReset()
    {
        Open = false;
        Animator.SetBool("Open", false);
        return true;
    }

    public override void Initialize()
    {
        _player = Player.Instance();
        TriggerWatcher.OnTriggerChanged+= TriggerWatcherOnOnTriggerChanged;
    }

    private void TriggerWatcherOnOnTriggerChanged(bool obj)
    {
        if (!obj) return;

        Collider.enabled = false;
        Open = true;
        Animator.SetBool("Open", true);
    }

    public override bool Activate()
    {
        _temporarilyOpen = Open = ActivationEvent != null && ActivationEvent.Source == TargetRoomExit;
        Animator.SetBool("Open", Open);
        Collider.enabled = !Open;
        if (_temporarilyOpen && !TriggerWatcher.Trigger.Triggered)
        {
            DefaultMachinery.AddBasicMachine(CheckDistanceToPlayer());
        }
        return true;
    }

    private IEnumerable<IEnumerable<Action>> CheckDistanceToPlayer()
    {
        while (_temporarilyOpen)
        {
            if (Vector2.Distance(transform.position, _player.transform.position) > DistanceBeforeClosing)
            {
                _temporarilyOpen = false;
            }
            yield return TimeYields.WaitOneFrameX;
        }

        Collider.enabled = true;
        Open = false;
        Animator.SetBool("Open", false);
    }
}
