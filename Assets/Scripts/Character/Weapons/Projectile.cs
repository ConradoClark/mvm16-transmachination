using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
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

    private LichtPhysics _physics;
    private BasicMachinery<object> _machinery;
    private EffectsManager _effectsManager;

    protected void Awake()
    {
        _machinery = DefaultMachinery.GetDefaultMachinery();
        _physics = this.GetLichtPhysics();
        _effectsManager = EffectsManager.GetInstance();
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
        if (!_physics.GetCollisionState(PhysicsObject).Horizontal.TriggeredHit) yield break;

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
