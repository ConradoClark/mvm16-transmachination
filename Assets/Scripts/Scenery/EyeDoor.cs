using UnityEngine;

public class EyeDoor : RoomObject
{
    public ScriptableTriggerWatcher TriggerWatcher;
    public Animator Animator;
    public Animator HandleAnimator;
    public RoomExit TargetRoomExit;
    public Collider2D Collider;
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
        if (TriggerWatcher.Trigger.Triggered)
        {
            Collider.enabled = false;
            HandleAnimator.gameObject.SetActive(false);
        }
    }

    private void TriggerWatcher_OnTriggerChanged(bool obj)
    {
        if (TriggerWatcher.Trigger.Triggered)
        {
            Collider.enabled = false;
            HandleAnimator.gameObject.SetActive(false);
        }
    }

    public override bool Activate()
    {
        HandleAnimator.speed = TriggerWatcher.Trigger.Triggered ? 1 : 0;
        Animator.speed = TriggerWatcher.Trigger.Triggered ? 0 : 1;

        if (TriggerWatcher.Trigger.Triggered)
        {
            Collider.enabled = false;
            HandleAnimator.gameObject.SetActive(false);
        }

        return true;
    }
}
