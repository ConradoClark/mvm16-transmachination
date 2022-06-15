using UnityEngine;

[CreateAssetMenu(fileName = "AllTriggers", menuName = "TM/Saves/AllTriggers", order = 1)]
public class AllTriggers : ScriptableObject
{
    public ScriptableTrigger[] Triggers;
}
