using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Pooling;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathEffects : BaseUIObject
{
    public SpriteRenderer DeathScreenCover;

    private Player _player;
    private EffectsManager _effectsManager;
    private DeathParticlePool _deathParticles;
    private PlayerInput _playerInput;

    public float ParticleSpeed;
    public float ParticleDurationInSeconds;
    public int AmountOfParticles;
    public Transform DeathOptions;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _player = Player.Instance();
        _effectsManager = EffectsManager.Instance();
        _deathParticles = _effectsManager.DeathParticlePool;
        _playerInput = PlayerInput.GetPlayerByIndex(0);
    }
    public void ShowDeath()
    {
        DefaultMachinery.AddBasicMachine(Death());
    }

    private IEnumerable<IEnumerable<Action>> Death()
    {
        _player.BlockAllMovement(this);

        var index = 0;
        if (AmountOfParticles > 0)
        {
            if (_deathParticles.TryGetManyFromPool(AmountOfParticles, out var objects))
            {
                foreach (var obj in objects)
                {
                    obj.transform.position = _player.transform.position;
                    obj.Speed = ParticleSpeed;
                    obj.DurationInSeconds = ParticleDurationInSeconds;

                    var angle = Mathf.Deg2Rad * (index * 360f / AmountOfParticles);
                    obj.Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
                    index++;
                }
            }
        }

        yield return TimeYields.WaitSeconds(UITimer, 0.1);

        if (AmountOfParticles > 0)
        {
            if (_deathParticles.TryGetManyFromPool(AmountOfParticles, out var objects))
            {
                foreach (var obj in objects)
                {
                    obj.transform.position = _player.transform.position;
                    obj.Speed = ParticleSpeed;
                    obj.DurationInSeconds = ParticleDurationInSeconds;

                    var angle = Mathf.Deg2Rad * ((360f / AmountOfParticles*0.25f) + index * 360f / AmountOfParticles);
                    obj.Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
                    index++;
                }
            }
        }

        yield return TimeYields.WaitSeconds(UITimer, 0.1);

        if (AmountOfParticles > 0)
        {
            if (_deathParticles.TryGetManyFromPool(AmountOfParticles, out var objects))
            {
                foreach (var obj in objects)
                {
                    obj.transform.position = _player.transform.position;
                    obj.Speed = ParticleSpeed;
                    obj.DurationInSeconds = ParticleDurationInSeconds;

                    var angle = Mathf.Deg2Rad * ((360f / AmountOfParticles * 0.75f) + index * 360f / AmountOfParticles);
                    obj.Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
                    index++;
                }
            }
        }

        _player.Hide();

        yield return TimeYields.WaitSeconds(UITimer, 1);

        DeathScreenCover.color = new Color(0, 0, 0, 0);
        DeathScreenCover.enabled = true;
        yield return DeathScreenCover.GetAccessor()
            .Color
            .A
            .SetTarget(1f)
            .Over(2f)
            .UsingTimer(UITimer)
            .Easing(EasingYields.EasingFunction.CubicEaseInOut)
            .Build();

        _playerInput.SwitchCurrentActionMap("UI");
        DeathOptions.gameObject.SetActive(true);

        _player.Show();
    }

    public IEnumerable<IEnumerable<Action>> Respawn()
    {
        _playerInput.SwitchCurrentActionMap("Character");
        DeathOptions.gameObject.SetActive(false);
        yield return DeathScreenCover.GetAccessor()
            .Color
            .A
            .SetTarget(0f)
            .Over(2f)
            .UsingTimer(UITimer)
            .Easing(EasingYields.EasingFunction.CubicEaseInOut)
            .Build();
        DeathScreenCover.enabled = false;
    }
}

