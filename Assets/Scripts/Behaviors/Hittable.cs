using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;

public class Hittable<T> : MonoBehaviour where T : class
{
    public Collider2D Collider;
    public float TriggerFrequency;

    private BasicMachinery<object> _defaultMachinery;
    private ITime _gameTimer;
    private LichtPhysics _physics;

    public event Action<HitEventArgs> OnHit;

    public class HitEventArgs
    {
        public CollisionTrigger Trigger;
        public T DamageComponent;
    }

    private void Awake()
    {
        _defaultMachinery = DefaultMachinery.GetDefaultMachinery();
        _physics = this.GetLichtPhysics();
        _gameTimer = DefaultGameTimer.GetTimer();
    }

    private void OnEnable()
    {
        _defaultMachinery.AddBasicMachine(CheckCollision());
    }

    private IEnumerable<IEnumerable<Action>> CheckCollision()
    {
        while (isActiveAndEnabled)
        {
            if (_physics.CheckCollision(Collider, out var trigger) && trigger.Actor.TryGetCustomObject<T>(out var damageSource))
            {
                OnHit?.Invoke(new HitEventArgs()
                {
                    Trigger = trigger,
                    DamageComponent = damageSource,
                });
                yield return TimeYields.WaitMilliseconds(_gameTimer, TriggerFrequency);
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
