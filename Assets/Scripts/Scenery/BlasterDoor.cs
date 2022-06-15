using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;

public class BlasterDoor : RoomObject
{
    public BlasterHittable HitBox;
    public Animator Animator;
    public bool Open;
    public RoomExit TargetRoomExit;

    private Player _player;
    private bool _temporarilyOpen;

    public override void PerformDestroy()
    {
        HitBox.OnHit -= HitBox_OnHit;
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
        return true;
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

    private void HitBox_OnHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        HitBox.Collider.enabled = false;
        Open = true;
        Animator.SetBool("Open", true);
    }
}
