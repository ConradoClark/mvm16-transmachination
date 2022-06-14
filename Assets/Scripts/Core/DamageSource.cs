using Licht.Impl.Memory;
using Licht.Unity.Physics;
using UnityEditor;
using UnityEditorInternal;
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
    public bool Enabled;

    private readonly Caterpillar<bool> _triggered = new Caterpillar<bool>()
    {
        TailSize = 1
    };

    public void Trigger(bool result)
    {
        _triggered.Current = result;
    }

    public bool Triggered => _triggered.Current;

    private void Awake()
    {
        if (Source == null) return;
        Source.AddCustomObject(this);
    }
}
