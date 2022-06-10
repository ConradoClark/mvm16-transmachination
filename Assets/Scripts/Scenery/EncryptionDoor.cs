using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEditorInternal;
using UnityEngine;

public class EncryptionDoor : RoomObject
{
    public EncryptionWheel Wheel;
    public BlasterHittable HitBox;
    public Animator Animator;
    public bool Open;
    public RoomExit TargetRoomExit;

    private Player _player;
    private bool _temporarilyOpen;

    private bool _animating;
    private bool _hooked;

    public override void PerformDestroy()
    {
        HitBox.OnHit -= HitBox_OnHit;
    }

    public override bool PerformReset()
    {
        _hooked = false;
        Open = false;
        Animator.SetBool("Open", false);
        return true;
    }

    public override void Initialize()
    {
        _player = Player.Instance();
        HitBox.OnHit += HitBox_OnHit;
    }

    public override bool Activate()
    {
        _temporarilyOpen = Open = ActivationEvent != null && ActivationEvent.Source == TargetRoomExit;
        Animator.SetBool("Open", Open);
        HitBox.Collider.enabled = !Open;
        if (_temporarilyOpen)
        {
            DefaultMachinery.AddBasicMachine(CheckDistanceToPlayer());
        }

        if (!Open) Wheel.HideInstantly();

        DefaultMachinery.AddBasicMachine(HandleDoor());

        return true;
    }

    private IEnumerable<IEnumerable<Action>> HandleDoor()
    {
        if (_hooked) yield break;
        _hooked = true;
        while (isActiveAndEnabled)
        {
            if (Open) yield return TimeYields.WaitOneFrameX;

            if (!Open && !Wheel.Hidden && Vector2.Distance(transform.position, _player.transform.position) > 3f)
            {
                yield return Wheel.Hide().AsCoroutine();
            }

            if (!Open && Wheel.Hidden && Vector2.Distance(transform.position, _player.transform.position) < 3f)
            {
                yield return Wheel.Show().AsCoroutine();
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> CheckDistanceToPlayer()
    {
        while (_temporarilyOpen)
        {
            if (Vector2.Distance(transform.position, _player.transform.position) > 3f)
            {
                _temporarilyOpen = false;
            }
            yield return TimeYields.WaitOneFrameX;
        }
            
        HitBox.Collider.enabled = true;
        Open = false;
        Animator.SetBool("Open", false);
    }

    private IEnumerable<IEnumerable<Action>> CheckHit()
    {
        if (Wheel.Hidden) yield break;

        _animating = true;
        yield return Wheel.Hit().AsCoroutine();

        if (Wheel.HitOnQuadrant)
        {
            HitBox.Collider.enabled = false;
            Open = true;
            Animator.SetBool("Open", true);
            yield return Wheel.Hide().AsCoroutine();
        }

        _animating = false;
    }

    private void HitBox_OnHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        if (_animating) return;

        DefaultMachinery.AddBasicMachine(CheckHit());
    }
}
