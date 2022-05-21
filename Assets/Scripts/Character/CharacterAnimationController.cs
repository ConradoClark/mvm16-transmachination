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

    private LichtPhysics _physics;
    private bool _afterJump;

    private void Awake()
    {
        _physics = this.GetLichtPhysics();
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

        DefaultMachinery.GetDefaultMachinery().AddBasicMachine(HandleOnGround());
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
        if (obj.Direction>0)
        {
            Animator.SetBool("FacingRight", true);
        }
        else if (obj.Direction <0)
        {
            Animator.SetBool("FacingRight", false);
        }
    }
    private void OnJumpEnd(LichtPlatformerJumpController.LichtPlatformerJumpEventArgs obj)
    {
        if (obj.Source.Target != Target) return;
        Animator.SetBool("IsJumping", false);
        _afterJump = true;
    }

    private void OnJumpStart(LichtPlatformerJumpController.LichtPlatformerJumpEventArgs obj)
    {
        if (obj.Source.Target != Target) return;
        Animator.SetBool("IsJumping", true);
        Animator.SetBool("OnGround", false);
    }
}

