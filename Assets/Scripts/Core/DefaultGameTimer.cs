using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Objects;
using UnityEngine;

public class DefaultGameTimer : MonoBehaviour
{
    public TimerScriptable TimerRef;

    public static ITime GetTimer()
    {
        var obj = FindObjectOfType<DefaultGameTimer>();
        return obj.TimerRef.Timer;
    }
}
