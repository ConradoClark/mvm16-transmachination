using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableFormComposition", menuName = "TM/Triggers/ScriptableFormComposition", order = 1)]
public class ScriptableFormComposition : ScriptableObject
{
    public ScriptableForm Arms;
    public ScriptableForm Eyes;
    public ScriptableForm Legs;
}
