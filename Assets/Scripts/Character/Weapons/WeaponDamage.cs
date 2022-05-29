using Licht.Unity.Physics;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public LichtPhysicsObject Source;
    public ScriptableDamage Damage;

    private void Awake()
    {
        Source.AddCustomObject(this);
    }
}
