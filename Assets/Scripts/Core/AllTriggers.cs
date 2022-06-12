using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AllTriggers", menuName = "TM/Saves/AllTriggers", order = 1)]
public class AllTriggers : ScriptableObject
{
    public ScriptableTrigger[] Triggers;
}
