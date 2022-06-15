using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;
using UnityEngine;

public class Projectile : EffectPoolable
{
    public string EffectOnCollision;
    public LichtPhysicsObject PhysicsObject;
    public Vector2 Direction;
    public float Speed;
    public float MaximumTimeInMs;
    public SpriteRenderer SpriteRenderer;

    public LichtPhysicsObject Source { get; set; }

    private LichtPhysics _physics;
    private BasicMachinery<object> _machinery;
    private EffectsManager _effectsManager;
    private IEventPublisher<WeaponEvents, WeaponEventArgs> _eventPublisher;

    protected void Awake()
    {
        _machinery = DefaultMachinery.GetDefaultMachinery();
        _physics = this.GetLichtPhysics();
        _effectsManager = EffectsManager.Instance();
        _eventPublisher = this.RegisterAsEventPublisher<WeaponEvents, WeaponEventArgs>();
    }
    public override void OnActivation()
    {   
        _machinery.AddBasicMachine(HandleProjectile());
    }

    private IEnumerable<IEnumerable<Action>> HandleProjectile()
    {
        yield return TimeYields.WaitOneFrameX;
        SpriteRenderer.flipX = Direction.normalized.x < 0;
        while (!IsEffectOver)
        {
            if (MaximumTimeInMs > 0)
            {
                foreach (var _ in TimeYields.WaitMilliseconds(DefaultGameTimer.GetTimer(), MaximumTimeInMs))
                {
                    PhysicsObject.ApplySpeed(Direction.normalized * Speed *
                                             (float)DefaultGameTimer.GetTimer().UpdatedTimeInMilliseconds
                                             * 0.001f);
                    yield return HandleCollision().AsCoroutine();
                    if (IsEffectOver) break;
                    yield return TimeYields.WaitOneFrameX;
                }

                IsEffectOver = true;
            }
            else
            {
                PhysicsObject.ApplySpeed((float) DefaultGameTimer.GetTimer().UpdatedTimeInMilliseconds
                                         * 0.001f * Speed * Direction.normalized);
                yield return HandleCollision().AsCoroutine();
                if (!IsEffectOver) yield return TimeYields.WaitOneFrameX;
            }
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleCollision()
    {
        var collisionState = _physics.GetCollisionState(PhysicsObject);
        if (!collisionState.Horizontal.TriggeredHit
            && (collisionState.Custom?.All(c=>!c.TriggeredHit) ?? true)) yield break;


        if (collisionState.Horizontal.Hits.Any(c => _physics.IsSemiSolid(c.collider))) yield break;

        _eventPublisher.PublishEvent(WeaponEvents.OnImpact,
            new WeaponEventArgs { Source = Source, Projectile = this });

        if (!string.IsNullOrWhiteSpace(EffectOnCollision))
        {
            var pool = _effectsManager.GetEffect(EffectOnCollision);

            if (pool!= null && pool.TryGetFromPool(out var obj))
            {
                obj.Component.transform.position = transform.position;
            }

        }

        IsEffectOver = true;
    }

    public override bool IsEffectOver { get; protected set; }
}
