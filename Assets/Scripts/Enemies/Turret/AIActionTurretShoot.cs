using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;

public class AIActionTurretShoot : AITimedAction
{
    public string ShootAnimationParam;
    public Animator Animator;

    public override IEnumerable<IEnumerable<Action>> Run()
    {
        Animator.SetBool(ShootAnimationParam, true);
        yield return TimeYields.WaitSeconds(GameTimer, DurationInSeconds);
        Animator.SetBool(ShootAnimationParam, false);
    }
}
