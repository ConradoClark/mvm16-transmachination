using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;

public class AIActionBossIdle : AIAction
{
    public string IdleAnimatorParam;
    public float WaitTimeInSeconds;
    public LichtPhysicsObject BossPhysicsObject;
    public float MovementMagnitude;

    public override IEnumerable<IEnumerable<Action>> Run()
    {
        var x1= BossPhysicsObject.GetSpeedAccessor(Vector2.zero)
            .X
            .SetTarget(MovementMagnitude *0.5f)
            .Over(WaitTimeInSeconds*0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(GameTimer)
            .Build();

        var y1 = BossPhysicsObject.GetSpeedAccessor(Vector2.zero)
            .Y
            .SetTarget(MovementMagnitude * 0.5f)
            .Over(WaitTimeInSeconds * 0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();

        var x2 = BossPhysicsObject.GetSpeedAccessor(Vector2.zero)
            .X
            .SetTarget(-MovementMagnitude * 0.5f)
            .Over(WaitTimeInSeconds * 0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        var y2 = BossPhysicsObject.GetSpeedAccessor(Vector2.zero)
            .Y
            .SetTarget(-MovementMagnitude * 0.5f)
            .Over(WaitTimeInSeconds * 0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();

        yield return x1.Combine(y1);
        yield return x1.Combine(y2);
        yield return x2.Combine(y2);
        yield return x2.Combine(y1);
    }
}
