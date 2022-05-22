using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Blaster : MonoBehaviour
{
    public ScriptableInputAction MainWeapon;
    public float CooldownInMs;
    public ProjectilePool BlasterPool;
    public Vector2 Offset;
    public LichtPhysicsObject CharacterPhysicsObject;
    public float ProjectileSpeed;
    public ScriptableFormComposition CharacterForm;

    private LichtPhysics _physics;
    private PlayerInput _input;
    private IEventPublisher<WeaponEvents, WeaponEventArgs> _eventPublisher;

    private void Awake()
    {
        _physics = this.GetLichtPhysics();
        _input = PlayerInput.GetPlayerByIndex(0);
    }

    private void OnEnable()
    {
        DefaultMachinery.GetDefaultMachinery().AddBasicMachine(HandleShooting());
        _eventPublisher = this.RegisterAsEventPublisher<WeaponEvents, WeaponEventArgs>();
    }

    private void OnDisable()
    {
        this.UnregisterAsEventPublisher<WeaponEvents, WeaponEventArgs>();
    }

    private IEnumerable<IEnumerable<Action>> HandleShooting()
    {
        var blasterInput = _input.actions[MainWeapon.ActionName];
        while (isActiveAndEnabled)
        {
            if (CharacterForm.Arms.Form == ScriptableForm.CharacterForm.Robot && blasterInput.WasPerformedThisFrame())
            {
                if (BlasterPool.TryGetFromPool(out var obj))
                {
                    obj.Component.transform.position = transform.position + (Vector3)Offset;
                    obj.Direction =
                        new Vector2(CharacterPhysicsObject.LatestDirection.x >= 0 ? 1 : -1,
                            -0.1f + Random.value * 0.2f);
                    obj.Speed = ProjectileSpeed;

                    _eventPublisher.PublishEvent(WeaponEvents.OnShoot,
                        new WeaponEventArgs { Source = CharacterPhysicsObject, Projectile = obj });
                }

                yield return TimeYields.WaitMilliseconds(DefaultGameTimer.GetTimer(), CooldownInMs);
            }
            else yield return TimeYields.WaitOneFrameX;
        }
    }
}
