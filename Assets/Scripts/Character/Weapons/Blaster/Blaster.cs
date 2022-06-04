using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Blaster : BaseMovementController
{
    public ScriptableInputAction MainWeapon;
    public float CooldownInMs;
    public ProjectilePool BlasterPool;
    public Vector2 Offset;
    public float ProjectileSpeed;
    public ScriptableFormComposition CharacterForm;
    public float RecoilStrength;

    private PlayerInput _input;
    private IEventPublisher<WeaponEvents, WeaponEventArgs> _eventPublisher;
    private Player _player;
    protected override void Awake()
    {
        base.Awake();
        _input = PlayerInput.GetPlayerByIndex(0);
        _player = Player.Instance();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleShooting());
        _eventPublisher = this.RegisterAsEventPublisher<WeaponEvents, WeaponEventArgs>();
    }

    private void OnDisable()
    {
        this.UnregisterAsEventPublisher<WeaponEvents, WeaponEventArgs>();
    }


    private IEnumerable<IEnumerable<Action>> Recoil(Vector2 direction)
    {
        yield return _player.PhysicsObject.GetSpeedAccessor()
            .X
            .Decrease(RecoilStrength * direction.x)
            .Over(CooldownInMs * 0.001f * 0.5f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(GameTimer)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> HandleShooting()
    {
        var blasterInput = _input.actions[MainWeapon.ActionName];
        while (isActiveAndEnabled)
        {
            if (!IsBlocked && CharacterForm.Arms.Form == ScriptableForm.CharacterForm.Robot && blasterInput.WasPerformedThisFrame())
            {
                _player.MoveController.BlockMovement(this);
                _player.JumpController.BlockMovement(this);
                if (BlasterPool.TryGetFromPool(out var obj))
                {
                    obj.Component.transform.position = transform.position + new Vector3(Offset.x * _player.MoveController.LatestDirection, Offset.y);
                    obj.Direction =
                        new Vector2(_player.MoveController.LatestDirection,
                            -0.05f + Random.value * 0.15f);
                    obj.Speed = ProjectileSpeed * (_player.DashController.IsDashing ? 1.5f: 1);

                    DefaultMachinery.AddBasicMachine(Recoil(new Vector2(obj.Direction.x,0)));

                    _eventPublisher.PublishEvent(WeaponEvents.OnShoot,
                        new WeaponEventArgs { Source = _player.PhysicsObject, Projectile = obj });
                }

                yield return TimeYields.WaitOneFrameX;
                _player.JumpController.UnblockMovement(this);

                yield return TimeYields.WaitMilliseconds(DefaultGameTimer.GetTimer(), CooldownInMs);
                _player.MoveController.UnblockMovement(this);
                
                
            }
            else yield return TimeYields.WaitOneFrameX;
        }
    }
}
