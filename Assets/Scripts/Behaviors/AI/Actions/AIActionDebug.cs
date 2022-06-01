using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;

public class AIActionDebug : AIAction
{
    public float WaitTimeInSeconds;

    public string DebugText;


    public override IEnumerable<IEnumerable<Action>> Run()
    {
        Debug.Log(DebugText);
        yield return TimeYields.WaitSeconds(GameTimer, WaitTimeInSeconds);
    }
}
