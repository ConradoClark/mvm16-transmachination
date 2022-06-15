using UnityEngine;

public class DashCrystalDoor : RoomObject
{
    public ScriptableTriggerWatcher TriggerWatcher;
    public Animator Animator;
    public Animator HandleAnimator;
    public RoomExit TargetRoomExit;
    private Player _player;
    public Collider2D[] Colliders;

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
        HandleAnimator.SetBool("Open", obj);
        ChangeColliderState(!obj);
    }

    public override bool Activate()
    {
        HandleAnimator.SetBool("Open", TriggerWatcher.Trigger.Triggered);
        ChangeColliderState(!TriggerWatcher.Trigger.Triggered);
        return true;
    }

    private void ChangeColliderState(bool state)
    {
        foreach (var col in Colliders)
        {
            col.enabled = state;
        }
    }
}
