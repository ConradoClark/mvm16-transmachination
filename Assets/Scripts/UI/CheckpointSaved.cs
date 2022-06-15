using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class CheckpointSaved : SceneObject<CheckpointSaved>
{
    public Vector3 Origin;
    public Vector3 Target;

    public AudioSource Audio;
    public float DurationInSeconds;
    private ITime _uiTimer;

    private void Awake()
    {
        _uiTimer = DefaultUITimer.GetTimer();
    }

    public IEnumerable<IEnumerable<Action>> ShowCheckpointSaved()
    {
        transform.position = Origin;

        Audio.Play();

        yield return transform.GetAccessor()
            .Position
            .Y
            .SetTarget(Target.y)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .UsingTimer(_uiTimer)
            .Build();

        yield return TimeYields.WaitSeconds(_uiTimer, DurationInSeconds);

        yield return transform.GetAccessor()
            .Position
            .Y
            .SetTarget(Origin.y)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .UsingTimer(_uiTimer)
            .Build();
    }
}
