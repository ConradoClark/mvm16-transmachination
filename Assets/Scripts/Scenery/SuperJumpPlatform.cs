using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using UnityEngine;

public class SuperJumpPlatform : RoomObject
{
    public Animator Animator;
    public ScriptableTriggerWatcher TriggerWatcher;
    public Light Light;
    public SpriteRenderer Platform;
    public SpriteRenderer Arrow;
    public Collider2D PlatformCollider;
    public float JumpSpeed;
    public float BoostTimeInMs;

    private Player _player;
    private float _originalLuminance;
    private bool _eventHooked;

    public override bool PerformReset()
    {
        SetState();
        return true;
    }

    public override void Initialize()
    {
        _player = Player.Instance();
        _originalLuminance = Platform.material.GetFloat("_Luminance");
    }

    public override bool Activate()
    {
        if (_eventHooked) return false;

        _eventHooked = true;
        TriggerWatcher.OnTriggerChanged += TriggerWatcher_OnTriggerChanged;
        this.ObserveEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents,
            LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(
            LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart,
            OnEvent);
        return true;
    }

    private void OnEvent(LichtPlatformerJumpController.LichtPlatformerJumpEventArgs obj)
    {
        if (obj.Source != _player.JumpController || !TriggerWatcher.Trigger.Triggered) return;

        var filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = _player.LayerMask
        };
        var results = new Collider2D[1];
        if (PlatformCollider.OverlapCollider(filter, results) > 0)
        {
            DefaultMachinery.AddBasicMachine(Flash());
            DefaultMachinery.AddBasicMachine(JumpBoost());
        }
    }

    private IEnumerable<IEnumerable<Action>> Flash()
    {
        yield return new LerpBuilder(f => Platform.material.SetFloat("_Luminance", f),
                () => Platform.material.GetFloat("_Luminance"))
            .Over(0.5f)
            .SetTarget(0.85f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .Build();

        yield return new LerpBuilder(f => Platform.material.SetFloat("_Luminance", f),
                () => Platform.material.GetFloat("_Luminance"))
            .Over(0.5f)
            .SetTarget(_originalLuminance)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> JumpBoost()
    {
        yield return TimeYields.WaitMilliseconds(GameTimer, BoostTimeInMs, step =>
        {
            _player.PhysicsObject.ApplySpeed(new Vector2(0, (BoostTimeInMs - (float) step) * 0.001f * JumpSpeed * (float)GameTimer.UpdatedTimeInMilliseconds * 0.001f));
        });
    }

    public override bool Deactivate()
    {
        _eventHooked = false;
        TriggerWatcher.OnTriggerChanged -= TriggerWatcher_OnTriggerChanged;
        this.StopObservingEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents,
            LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(
            LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart,
            OnEvent);
        return true;
    }

    private void TriggerWatcher_OnTriggerChanged(bool obj)
    {
        SetState();
    }

    private void SetState()
    {
        Arrow.enabled = TriggerWatcher.Trigger.Triggered;
        Animator.speed = TriggerWatcher.Trigger.Triggered ? 1 : 0f;
        Light.intensity = TriggerWatcher.Trigger.Triggered ? 1 : 0f;
    }
}
