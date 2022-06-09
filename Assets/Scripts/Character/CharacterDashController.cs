using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CharacterDashController : BaseMovementController
{
    public float DashSpeed;
    public float DashDurationInSeconds;
    public float DashDecelerationInSeconds;
    public float MidAirDashDecelerationInSeconds;
    public float DashCooldownInSeconds;
    public ScriptableTrigger AllowAirDash;
    public ScriptableLichtForceIdentifier Gravity;
    public SpriteRenderer DashEffect;
    public ScriptableInputAction Dash;
    public DamageSource DashBash;
    public ScriptableTrigger AllowDashBash;
    public Color DashBashColor;

    private readonly Color _transparent = new Color(0, 0, 0, 0);

    private PlayerInput _input;
    private Player _player;
    private LichtPhysics _physics;
    private bool _flicker;
    private EffectsManager _effectsManager;
    private PrefabPool _dashParticles;
    private PrefabPool _dashBashParticles;
    public bool IsDashing { get; private set; }
    private bool _bashing;

    public float RecoilDuration;
    public float RecoilYValue;
    public float RecoilSpeed;
    public Transform DashRecoilLock;

    public enum DashEvents
    {
        OnDashStart,
        OnDashEnd,
    }

    private IEventPublisher<DashEvents> _eventPublisher;

    protected override void Awake()
    {
        base.Awake();
        _input = PlayerInput.GetPlayerByIndex(0);
        _player = Player.Instance();
        _eventPublisher = this.RegisterAsEventPublisher<DashEvents>();
        _physics = this.GetLichtPhysics();
        _effectsManager = EffectsManager.Instance();
        _dashParticles = _effectsManager.GetEffect("DashParticle");
        _dashBashParticles = _effectsManager.GetEffect("DashBashParticle");
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleDashing());
        this.ObserveEvent<Hittable<DamageSource>.HitEvents, Hittable<DamageSource>.HitEventArgs>(Hittable<DamageSource>.HitEvents.OnHit, OnDashBashHit);
    }

    private void OnDisable()
    {
        this.StopObservingEvent<Hittable<DamageSource>.HitEvents, Hittable<DamageSource>.HitEventArgs>(Hittable<DamageSource>.HitEvents.OnHit, OnDashBashHit);
    }

    private void OnDashBashHit(Hittable<DamageSource>.HitEventArgs obj)
    {
        if (obj.DamageComponent != DashBash) return;
        DashBash.Enabled = false;

        DefaultMachinery.AddBasicMachine(DashRecoil());
    }

    private IEnumerable<IEnumerable<Action>> DashRecoil()
    {
        IsDashing = false;
        _bashing = true;
        var dir = new Vector2(-_player.MoveController.LatestDirection, RecoilYValue);

        var @lock = new object();
        _player.MoveController.BlockMovement(DashRecoilLock);

        yield return TimeYields.WaitSeconds(GameTimer, RecoilDuration, (time) =>
        {
            if (dir == Vector2.zero) dir = Vector2.right;

            var accel = (float)(RecoilDuration - time*0.001f);
            var latestSpeed = accel * 0.001f * RecoilSpeed * (float)GameTimer.UpdatedTimeInMilliseconds * dir;
            _player.PhysicsObject.ApplySpeed(latestSpeed);
        }, () => IsBlocked);

        _player.MoveController.UnblockMovement(DashRecoilLock);
        _bashing = false;
    }

    private float GetDashSpeed()
    {
        return AllowDashBash.Triggered ? DashSpeed + 2f : DashSpeed;
    }

    private IEnumerable<IEnumerable<Action>> PerformDashMovement(LichtCustomPhysicsForce gravity)
    {
        var dir = new Vector2(_player.MoveController.LatestDirection, 0);
        DashEffect.flipX = Mathf.Sign(dir.x) < 0;
        var latestSpeed = Vector2.zero;
        yield return TimeYields.WaitSeconds(GameTimer, DashDurationInSeconds, (time) =>
        {
            if (dir == Vector2.zero) dir = Vector2.right;
            latestSpeed = 0.001f * GetDashSpeed() * (float)GameTimer.UpdatedTimeInMilliseconds * dir;
            _player.PhysicsObject.ApplySpeed(latestSpeed);
        }, () => IsBlocked || _bashing);

        _player.MoveController.UnblockMovement(this);
        gravity.UnblockForceFor(this, _player.PhysicsObject);

        yield return _player.PhysicsObject.GetSpeedAccessor(latestSpeed)
            .X
            .SetTarget(0)
            .Over(_physics.GetCollisionState(_player.PhysicsObject).Vertical.HitNegative ?
                DashDecelerationInSeconds : MidAirDashDecelerationInSeconds)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .BreakIf(()=> _bashing)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> FlickerDashEffect()
    {
        const string luminance = "_Luminance";
        while (_flicker)
        {
            DashEffect.material.SetFloat(luminance, 0.75f);
            yield return TimeYields.WaitMilliseconds(GameTimer, 25);
            DashEffect.material.SetFloat(luminance, 0f);
            yield return TimeYields.WaitMilliseconds(GameTimer, 25);
        }
        DashEffect.material.SetFloat(luminance, 0f);
    }

    private IEnumerable<IEnumerable<Action>> ShowDashEffect()
    {
        yield return TimeYields.WaitOneFrameX;
        const string opacity = "_Opacity";
        DashEffect.enabled = true;
        DashEffect.material.SetFloat(opacity, 0);

        DashEffect.material.SetColor("_ColorReplTarget1", AllowDashBash.Triggered ? DashBashColor : _transparent);
        DashEffect.material.SetFloat("_ColorReplTolerance1", AllowDashBash.Triggered ? 200f: 0f);

        _flicker = true;

        DefaultMachinery.AddBasicMachine(FlickerDashEffect());
        yield return new LerpBuilder(f => DashEffect.material.SetFloat(opacity, f),
                () => DashEffect.material.GetFloat(opacity))
            .SetTarget(1)
            .Over(DashDurationInSeconds * 0.5f)
            .UsingTimer(GameTimer)
            .BreakIf(() => IsBlocked || !IsDashing || _player.JumpController.IsJumping)
            .Easing(EasingYields.EasingFunction.CubicEaseInOut)
            .Build();

        yield return new LerpBuilder(f => DashEffect.material.SetFloat(opacity, f),
                () => DashEffect.material.GetFloat(opacity))
            .SetTarget(0)
            .Over(DashDurationInSeconds * 0.75f)
            .UsingTimer(GameTimer)
            .BreakIf(() => IsBlocked)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .Build();

        _flicker = DashEffect.enabled = !IsDashing;
    }

    private IEnumerable<IEnumerable<Action>> SummonParticles()
    {
        var pool = AllowDashBash.Triggered ? _dashBashParticles : _dashParticles;
        for (var i = 0; i < 4; i++)
        {
            if (pool.TryGetFromPool(out var obj))
            {
                obj.Component.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * 0.2f);
            }

            yield return TimeYields.WaitSeconds(GameTimer, DashDurationInSeconds * 0.3f);
        }
        
    }

    private IEnumerable<IEnumerable<Action>> HandleDashing()
    {
        var gravity = _physics.GetCustomPhysicsForce(Gravity.Name);
        var dashInput = _input.actions[Dash.ActionName];
        var performedAirDash = false;
        while (isActiveAndEnabled)
        {
            var onGround = _physics.GetCollisionState(_player.PhysicsObject).Vertical.HitNegative;
            var canDash = (AllowAirDash.Triggered && !performedAirDash) || onGround;
            if (onGround) performedAirDash = false;

            if (!IsBlocked && canDash && dashInput.WasPerformedThisFrame())
            {
                IsDashing = true;
                if (AllowDashBash.Triggered)
                {
                    DashBash.Enabled = true;
                }

                if (!onGround) performedAirDash = true;
                _flicker = false;
                _player.MoveController.BlockMovement(this);
                gravity.BlockForceFor(this, _player.PhysicsObject);

                _eventPublisher.PublishEvent(DashEvents.OnDashStart);

                DefaultMachinery.AddBasicMachine(SummonParticles());
                DefaultMachinery.AddBasicMachine(PerformDashMovement(gravity));
                DefaultMachinery.AddBasicMachine(ShowDashEffect());

                yield return TimeYields.WaitSeconds(GameTimer, DashDurationInSeconds,
                    breakCondition: () => IsBlocked);
                _eventPublisher.PublishEvent(DashEvents.OnDashEnd);
                IsDashing = false;
                DashBash.Enabled = false;

                yield return TimeYields.WaitSeconds(GameTimer, DashCooldownInSeconds);
            }
            else yield return TimeYields.WaitOneFrameX;
        }
    }
}
