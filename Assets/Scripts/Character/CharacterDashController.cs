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
    private PlayerInput _input;
    private Player _player;
    private LichtPhysics _physics;
    private bool _flicker;
    private EffectsManager _effectsManager;
    private PrefabPool _dashParticles;
    public bool IsDashing { get; private set; }

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
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleDashing());
    }

    private IEnumerable<IEnumerable<Action>> PerformDashMovement(LichtCustomPhysicsForce gravity)
    {
        var dir = new Vector2(_player.MoveController.LatestDirection, 0);
        DashEffect.flipX = Mathf.Sign(dir.x) < 0;
        var latestSpeed = Vector2.zero;
        yield return TimeYields.WaitSeconds(GameTimer, DashDurationInSeconds, (time) =>
        {
            if (dir == Vector2.zero) dir = Vector2.right;
            latestSpeed = 0.001f * DashSpeed * (float)GameTimer.UpdatedTimeInMilliseconds * dir;
            _player.PhysicsObject.ApplySpeed(latestSpeed);
        }, () => IsBlocked);

        _player.MoveController.UnblockMovement(this);
        gravity.UnblockForceFor(this, _player.PhysicsObject);

        yield return _player.PhysicsObject.GetSpeedAccessor(latestSpeed)
            .X
            .SetTarget(0)
            .Over(_physics.GetCollisionState(_player.PhysicsObject).Vertical.HitNegative ?
                DashDecelerationInSeconds : MidAirDashDecelerationInSeconds)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
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
        for (var i = 0; i < 4; i++)
        {
            if (_dashParticles.TryGetFromPool(out var obj))
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

                yield return TimeYields.WaitSeconds(GameTimer, DashCooldownInSeconds);
            }
            else yield return TimeYields.WaitOneFrameX;
        }
    }
}
