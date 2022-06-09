using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public override bool PerformReset()
    {
        transform.position = _originalPosition;
        _enemy.PerformReset();
        return true;
    }

    public override void Initialize()
    {
        _originalPosition = transform.position;
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
            _physicsObject.ApplySpeed(Direction * Speed * (float) GameTimer.UpdatedTimeInMilliseconds * 0.001f);
            var collisionState = _physics.GetCollisionState(_physicsObject);

            if (collisionState.Horizontal.HitNegative)
            {
                Direction = Vector2.right;
            }

            if (collisionState.Horizontal.HitPositive)
            {
                Direction = Vector2.left;
            }

            if (collisionState.Vertical.HitNegative)
            {
                Direction = Vector2.up;
            }

            if (collisionState.Vertical.HitPositive)
            {
                Direction = Vector2.down;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    } 
    
}
