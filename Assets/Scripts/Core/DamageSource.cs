using Licht.Unity.Physics;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public enum DamageSourceType
    {
        Player,
        Enemy,
        Door
    }

    public LichtPhysicsObject Source;
    public DamageSourceType SourceType;
    public ScriptableDamage Damage;

    private void Awake()
    {
        Source.AddCustomObject(this);
    }
}
