using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction : BaseObject
{
    public abstract IEnumerable<IEnumerable<Action>> Run();
    public AIAction Next;
}
