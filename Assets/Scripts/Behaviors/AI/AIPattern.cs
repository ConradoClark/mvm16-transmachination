using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Generation;

[Serializable]
public class AIPattern : IWeighted<float>
{
    public AIAction Action;
    public AICondition[] Triggers;
    public int PatternWeight;
    public float Weight => PatternWeight;
    public bool IsAbsolute;
}

