using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Objects;
using UnityEngine;

public class DefaultGameTimer : SceneObject<DefaultGameTimer>
{
    public TimerScriptable TimerRef;

    public static ITime GetTimer()
    {
        return Instance().TimerRef.Timer;
    }
}
