using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;

public class AIActionIdle : AIAction
{
    public string IdleAnimatorParam;
    public Animator Animator;
    public float WaitTimeInSeconds;
    public override IEnumerable<IEnumerable<Action>> Run()
    {
        if (Animator != null)
        {
            Animator.SetBool(IdleAnimatorParam, true);
        }

        yield return TimeYields.WaitSeconds(GameTimer, WaitTimeInSeconds);

        if (Animator != null)
        {
            Animator.SetBool(IdleAnimatorParam, false);
        }
    }
}
