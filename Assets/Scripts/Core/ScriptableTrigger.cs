using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableTrigger", menuName = "TM/Triggers/ScriptableTrigger", order = 1)]
public class ScriptableTrigger : ScriptableObject
{
    [field: SerializeField]
    public virtual bool Triggered { get; set; }

    public TriggerType Type; 

    public enum TriggerType
    {
        Default,
        ArmorUp,
        EnergyUp,
    }
}
