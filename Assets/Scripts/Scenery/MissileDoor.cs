using UnityEngine;

public class MissileDoor : RoomObject
{
    public ScriptableTriggerWatcher TriggerWatcher;
    public Animator Animator;
    public Animator HandleAnimator;
    public RoomExit TargetRoomExit;
    private Player _player;

    public override void PerformDestroy()
    {
        TriggerWatcher.OnTriggerChanged -= TriggerWatcher_OnTriggerChanged;
    }

    public override bool PerformReset()
    {
        return true;
    }

    public override void Initialize()
    {
        _player = Player.Instance();
        TriggerWatcher.OnTriggerChanged += TriggerWatcher_OnTriggerChanged;
    }

    private void TriggerWatcher_OnTriggerChanged(bool obj)
    {
        
    }

    public override bool Activate()
    {
        HandleAnimator.speed = TriggerWatcher.Trigger.Triggered ? 1 : 0;
        Animator.speed = TriggerWatcher.Trigger.Triggered ? 0 : 1;

        return true;
    }
}
