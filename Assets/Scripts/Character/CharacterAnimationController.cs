using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator Animator;
    public LichtPhysicsObject Target;
    public TimerScriptable GameTimer;
    public SpriteRenderer[] Parts;
    public DamageHittable PlayerHit;
    public Player Player;

    private LichtPhysics _physics;
    private BasicMachinery<object> _machinery;
    private Stack<bool> _shootStack;

    private void Awake()
    {
        _physics = this.GetLichtPhysics();
        _machinery = DefaultMachinery.GetDefaultMachinery();
        _shootStack = new Stack<bool>();
    }

    private void OnDisable()
    {
        this.StopObservingEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnTurn, OnTurn);

        this.StopObservingEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStartMoving, OnStartMoving);

        this.StopObservingEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStopMoving, OnStopMoving);

        this.StopObservingEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(
            LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart, OnJumpStart);

        this.StopObservingEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(
            LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpEnd, OnJumpEnd);

        this.StopObservingEvent<WeaponEvents, WeaponEventArgs>(WeaponEvents.OnShoot, OnShoot);

        PlayerHit.OnHit -= PlayerHit_OnHit;
    }

    private void OnEnable()
    {
        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnTurn, OnTurn);

        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStartMoving, OnStartMoving);

        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStopMoving, OnStopMoving);

        this.ObserveEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(
            LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart, OnJumpStart);

        this.ObserveEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(
            LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpEnd, OnJumpEnd);

        this.ObserveEvent<WeaponEvents, WeaponEventArgs>(WeaponEvents.OnShoot, OnShoot);

        PlayerHit.OnHit += PlayerHit_OnHit;

       _machinery.AddBasicMachine(HandleOnGround());
    }

    private void PlayerHit_OnHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        _machinery.AddBasicMachine(HandleHit());
    }

    private IEnumerable<IEnumerable<Action>> HandleHit()
    {
        Animator.SetBool("IsGettingHit", true);
        yield return TimeYields.WaitSeconds(GameTimer.Timer, Player.GettingHitDurationInSeconds);
        Animator.SetBool("IsGettingHit", false);
    }

    private void OnShoot(WeaponEventArgs obj)
    {
        if (obj.Source != Target) return;
        Animator.SetBool("IsShooting", true);

        _shootStack.Push(true);
        _machinery.AddBasicMachine(EndShooting());
    }

    private IEnumerable<IEnumerable<Action>> EndShooting()
    {
        yield return TimeYields.WaitMilliseconds(GameTimer.Timer, 100);
        _shootStack.Pop();
        if (_shootStack.Count == 0)
        {
            Animator.SetBool("IsShooting", false);
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleOnGround()
    {
        while (isActiveAndEnabled)
        {
            if (!_physics.GetCollisionState(Target).Vertical.HitNegative)
            {
                yield return TimeYields.WaitMilliseconds(GameTimer.Timer, 150);
                if (!_physics.GetCollisionState(Target).Vertical.HitNegative)
                {
                    Animator.SetBool("OnGround", false);
                }

                while (!_physics.GetCollisionState(Target).Vertical.HitNegative)
                {
                    yield return TimeYields.WaitOneFrameX;
                }
            }

            if (_physics.GetCollisionState(Target).Vertical.HitNegative)
            {
                Animator.SetBool("OnGround", true);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private void OnStopMoving(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Source.Target != Target) return;

        Animator.SetBool("IsMoving", false);
    }

    private void OnStartMoving(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Source.Target != Target) return;

        Animator.SetBool("IsMoving", true);
        AdjustDirection(obj);
    }

    private void OnTurn(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Source.Target != Target) return;

        AdjustDirection(obj);
    }

    private void AdjustDirection(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Direction > 0)
        {
            Animator.SetBool("FacingRight", true);
            foreach (var spr in Parts)
            {
                spr.flipX = false;
            }

        }
        else if (obj.Direction < 0)
        {
            Animator.SetBool("FacingRight", false);
            foreach (var spr in Parts)
            {
                spr.flipX = true;
            }
        }
    }
    private void OnJumpEnd(LichtPlatformerJumpController.LichtPlatformerJumpEventArgs obj)
    {
        if (obj.Source.Target != Target) return;
        Animator.SetBool("IsJumping", false);
    }

    private void OnJumpStart(LichtPlatformerJumpController.LichtPlatformerJumpEventArgs obj)
    {
        if (obj.Source.Target != Target) return;
        Animator.SetBool("IsJumping", true);
        Animator.SetBool("OnGround", false);
    }
}

