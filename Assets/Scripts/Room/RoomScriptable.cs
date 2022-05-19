using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomScriptable", menuName = "TM/Room/RoomScriptable", order = 1)]
public class RoomScriptable : ScriptableObject
{
    public RoomDefinition Value;
}
