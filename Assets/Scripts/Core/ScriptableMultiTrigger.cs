using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableMultiTrigger", menuName = "TM/Triggers/ScriptableMultiTrigger", order = 1)]
public class ScriptableMultiTrigger : ScriptableTrigger
{
    public ScriptableTrigger[] Triggers;

    public override bool Triggered
    {
        get
        {
            return Triggers.All(t => t.Triggered);
        }
        set
        {

        }
    }
}
