using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class Capsule : RoomObject
{
    public ScriptableTrigger Trigger;
    public SpriteRenderer BeamRenderer;
    public Collider2D Collider;
    public Color ActiveColor;
    public Color InactiveColor;
    public Color FlashColor;
    public Light WestCrystalLight;
    public Light EastCrystalLight;
    public Light CapsuleLight;
    public Light PlaqueLight;

    private Player _player;
    private bool _triggering;
    private PrefabPool _capsuleParticles;

    private IEnumerable<IEnumerable<Action>> HandleCapsule()
    {
        var filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = _player.LayerMask
        };
        var results = new Collider2D[1];

        while (!Trigger.Triggered)
        {
            if (Collider.OverlapCollider(filter, results) > 0)
            {
                _triggering = true;
                _player.BlockAllMovement(this);
                yield return AnimateCapsule().AsCoroutine();
                Trigger.Triggered = true;
                _player.UnblockAllMovement(this);
            }

            yield return TimeYields.WaitMilliseconds(GameTimer, 10);
        }
    }

    private IEnumerable<IEnumerable<Action>> FlickerCrystalLights()
    {
        while (_triggering)
        {
            WestCrystalLight.intensity = 0;
            EastCrystalLight.intensity = 1;
            yield return TimeYields.WaitMilliseconds(GameTimer, 100);
            WestCrystalLight.intensity = 1.5f;
            EastCrystalLight.intensity = 0;
            yield return TimeYields.WaitMilliseconds(GameTimer, 100);
        }
    }

    private IEnumerable<IEnumerable<Action>> SummonParticles()
    {
        while (_triggering)
        {
            if (_capsuleParticles.TryGetFromPool(out var particle))
            {
                particle.Component.transform.position = transform.position +
                                                        new Vector3(Random.insideUnitCircle.x * 0.5f,
                                                            -0.1f + Random.insideUnitCircle.y * 0.7f);
            }
            yield return TimeYields.WaitMilliseconds(GameTimer, 50);
        }
    }

    private IEnumerable<IEnumerable<Action>> AnimateCapsule()
    {
        // summon particles
        // blink lights
        yield return TimeYields.WaitOneFrameX;

        DefaultMachinery.AddBasicMachine(FlickerCrystalLights());
        DefaultMachinery.AddBasicMachine(SummonParticles());

        yield return BeamRenderer.GetAccessor()
            .Color
            .ToColor(InactiveColor)
            .SetTarget(1)
            .Over(5f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .Build();

        _triggering = false;

        CapsuleLight.intensity = 4f;
        BeamRenderer.color = FlashColor;

        var colorToInactiveAgain = BeamRenderer.GetAccessor()
            .Color
            .ToColor(InactiveColor)
            .SetTarget(1)
            .Over(1f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .Build();

        var lights = new LerpBuilder(f=> WestCrystalLight.intensity = EastCrystalLight.intensity = CapsuleLight.intensity = PlaqueLight.intensity = f,
            () => CapsuleLight.intensity)
            .SetTarget(0f)
            .Over(1f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .Build();

        yield return colorToInactiveAgain.Combine(lights);
    }

    public override bool PerformReset()
    {
        return true;
    }

    public override void Initialize()
    {
        _player = Player.Instance();
        _capsuleParticles = EffectsManager.Instance().GetEffect("CapsuleParticle");
    }

    public override bool Activate()
    {
        BeamRenderer.color = Trigger.Triggered ? InactiveColor : ActiveColor;

        if (!Trigger.Triggered)
        {
            DefaultMachinery.AddBasicMachine(HandleCapsule());
        }

        return true;
    }
}
