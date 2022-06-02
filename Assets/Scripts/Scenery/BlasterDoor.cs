using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterDoor : RoomObject
{
    public BlasterHittable HitBox;
    public Animator Animator;
    public bool Open;

    public RoomExit TargetRoomExit;

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
        HitBox.OnHit += HitBox_OnHit;
    }

    public override bool Activate()
    {
        Open = ActivationEvent != null && ActivationEvent.Source == TargetRoomExit;
        Animator.SetBool("Open", Open);
        HitBox.Collider.enabled = !Open;
        return true;
    }

    private void HitBox_OnHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        HitBox.Collider.enabled = false;
        Open = true;
        Animator.SetBool("Open", true);
    }
}
