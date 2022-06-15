using System;
using System.Collections.Generic;

public abstract class AIAction : BaseObject
{
    public abstract IEnumerable<IEnumerable<Action>> Run();
    public AIAction Next;
}
