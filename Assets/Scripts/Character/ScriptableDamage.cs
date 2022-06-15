using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableDamage", menuName = "TM/Metroidvania/ScriptableDamage", order = 1)]
public class ScriptableDamage : ScriptableObject
{
    public DamageType DamageType;
    public int DamageAmount;
}

