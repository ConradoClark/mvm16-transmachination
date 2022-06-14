using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;

public abstract class Hittable<T> : MonoBehaviour where T : class
{
    public Collider2D Collider;
    public float TriggerFrequencyInSeconds;

    private BasicMachinery<object> _defaultMachinery;
    private ITime _gameTimer;
    private LichtPhysics _physics;

    public event Action<HitEventArgs> OnHit;
    protected IEventPublisher<HitEvents, HitEventArgs> EventPublisher;

    public enum HitEvents
    {
        OnHit
    }

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
        EventPublisher = this.RegisterAsEventPublisher<HitEvents, HitEventArgs>();
    }

    private void OnEnable()
    {
        _defaultMachinery.AddBasicMachine(CheckCollision());
    }

    private IEnumerable<IEnumerable<Action>> CheckCollision()
    {
        while (isActiveAndEnabled)
        {
            var collision = _physics.CheckCollision(Collider, out var trigger);
            if (collision && trigger.Actor.TryGetCustomObject<T>(out var damageSource) 
                          && ValidateHitSource(damageSource))
            {
                var eventArgs = new HitEventArgs
                {
                    Trigger = trigger,
                    DamageComponent = damageSource,
                };

                OnHit?.Invoke(eventArgs);
                EventPublisher.PublishEvent(HitEvents.OnHit, eventArgs);

                yield return TimeYields.WaitSeconds(_gameTimer, TriggerFrequencyInSeconds);
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }

    public void ForceInvoke(HitEventArgs eventArgs)
    {
        OnHit?.Invoke(eventArgs);
    }

    public abstract bool ValidateHitSource(T hitSource);
}
