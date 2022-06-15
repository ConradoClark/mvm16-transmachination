using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(LichtPhysicsObject))]
public class BallEnemy : RoomObject
{
    public Vector2 Direction;
    public float Speed;

    private Enemy _enemy;
    private LichtPhysicsObject _physicsObject;
    private LichtPhysics _physics;
    private Vector3 _originalPosition;
    private Vector2 _originalDirection;

    public override bool PerformReset()
    {
        Direction = _originalDirection;
        transform.position = _originalPosition;
        _enemy.PerformReset();
        return true;
    }

    public override void Initialize()
    {
        _originalPosition = transform.position;
        _originalDirection = Direction;
    }

    public override bool Activate()
    {
        DefaultMachinery.AddBasicMachine(HandleBall());
        return true;
    }

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _enemy = GetComponent<Enemy>();
        _physicsObject = GetComponent<LichtPhysicsObject>();
        _physics = this.GetLichtPhysics();
    }

    private IEnumerable<IEnumerable<Action>> HandleBall()
    {
        while (isActiveAndEnabled)
        {
            _physicsObject.ApplySpeed(Direction * Speed * (float)GameTimer.UpdatedTimeInMilliseconds * 0.001f);
            var collisionState = _physics.GetCollisionState(_physicsObject);

            if (collisionState.Horizontal.HitNegative && Mathf.Abs(_originalDirection.x) > 0)
            {
                Direction = Vector2.right;
            }

            if (collisionState.Horizontal.HitPositive && Mathf.Abs(_originalDirection.x) > 0)
            {
                Direction = Vector2.left;
            }

            if (collisionState.Vertical.HitNegative && Mathf.Abs(_originalDirection.y) > 0)
            {
                Direction = Vector2.up;
            }

            if (collisionState.Vertical.HitPositive && Mathf.Abs(_originalDirection.y) > 0)
            {
                Direction = Vector2.down;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

}
