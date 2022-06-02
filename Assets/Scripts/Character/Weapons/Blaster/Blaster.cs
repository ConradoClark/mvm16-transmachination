using System;
using System.Collections.Generic;
using System.Dynamic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Interfaces.Time;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Blaster : LichtMovementController
{
    public ScriptableInputAction MainWeapon;
    public float CooldownInMs;
    public ProjectilePool BlasterPool;
    public Vector2 Offset;
    public LichtPhysicsObject CharacterPhysicsObject;
    public LichtPlatformerMoveController MoveController;
    public LichtPlatformerJumpController JumpController;
    public float ProjectileSpeed;
    public ScriptableFormComposition CharacterForm;
    public float RecoilStrength;

    private LichtPhysics _physics;
    private PlayerInput _input;
    private BasicMachinery<object> _defaultMachinery;
    private IEventPublisher<WeaponEvents, WeaponEventArgs> _eventPublisher;
    private ITime _gameTimer;

    protected override void Awake()
    {
        base.Awake();
        _physics = this.GetLichtPhysics();
        _input = PlayerInput.GetPlayerByIndex(0);
        _defaultMachinery = DefaultMachinery.GetDefaultMachinery();
        _gameTimer = DefaultGameTimer.GetTimer();
    }

    private void OnEnable()
    {
        _defaultMachinery.AddBasicMachine(HandleShooting());
        _eventPublisher = this.RegisterAsEventPublisher<WeaponEvents, WeaponEventArgs>();
    }

    private void OnDisable()
    {
        this.UnregisterAsEventPublisher<WeaponEvents, WeaponEventArgs>();
    }


    private IEnumerable<IEnumerable<Action>> Recoil(Vector2 direction)
    {
        yield return CharacterPhysicsObject.GetSpeedAccessor()
            .X
            .Decrease(RecoilStrength * direction.x)
            .Over(CooldownInMs * 0.001f * 0.5f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(_gameTimer)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> HandleShooting()
    {
        var blasterInput = _input.actions[MainWeapon.ActionName];
        while (isActiveAndEnabled)
        {
            if (!IsBlocked && CharacterForm.Arms.Form == ScriptableForm.CharacterForm.Robot && blasterInput.WasPerformedThisFrame())
            {
                MoveController.BlockMovement(this);
                JumpController.BlockMovement(this);
                if (BlasterPool.TryGetFromPool(out var obj))
                {
                    obj.Component.transform.position = transform.position + new Vector3(Offset.x * MoveController.LatestDirection, Offset.y);
                    obj.Direction =
                        new Vector2(MoveController.LatestDirection,
                            -0.05f + Random.value * 0.15f);
                    obj.Speed = ProjectileSpeed;

                    _defaultMachinery.AddBasicMachine(Recoil(new Vector2(obj.Direction.x,0)));

                    _eventPublisher.PublishEvent(WeaponEvents.OnShoot,
                        new WeaponEventArgs { Source = CharacterPhysicsObject, Projectile = obj });
                }

                yield return TimeYields.WaitOneFrameX;
                JumpController.UnblockMovement(this);

                yield return TimeYields.WaitMilliseconds(DefaultGameTimer.GetTimer(), CooldownInMs);
                MoveController.UnblockMovement(this);
                
                
            }
            else yield return TimeYields.WaitOneFrameX;
        }
    }
}
