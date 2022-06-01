using Licht.Unity.Physics;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public LichtPhysicsObject Source;
    public ScriptableDamage Damage;

    private void Awake()
    {
        Source.AddCustomObject(this);
    }
}
