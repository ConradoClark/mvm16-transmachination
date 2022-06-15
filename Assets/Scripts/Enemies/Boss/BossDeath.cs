using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossDeath : BaseObject
{
    public Enemy Enemy;
    public ScriptableTrigger Trigger;
    public ScriptableTrigger Trigger2;
    public RandomPitchAudio ExplosionFX;
    public AudioSource BossMusic;

    public float ExplosionArea;

    private EffectsManager _effects;
    private PrefabPool _explosion;
    private Letterbox _letterbox;
    private Player _player;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _effects = EffectsManager.Instance();
        _explosion = _effects.GetEffect("BossExplosion");
        _letterbox = Letterbox.Instance();
        _player = Player.Instance();
    }
    private void OnEnable()
    {
        Enemy.OnDeath += Enemy_OnDeath;
    }

    private void Enemy_OnDeath()
    {
        DefaultMachinery.AddBasicMachine(Death());
    }

    private void OnDisable()
    {
        Enemy.OnDeath -= Enemy_OnDeath;
    }

    private IEnumerable<IEnumerable<Action>> Death()
    {
        BossMusic.Stop();

        DefaultMachinery.AddBasicMachine(_letterbox.ShowLetterbox());

        var max = 20;
        for (var i = 0; i < max; i++)
        {
            if (i % 2 == 0) ExplosionFX.Play();
            SpawnExplosion();
            yield return TimeYields.WaitMilliseconds(GameTimer, (max - i) * 15f);
        }

        yield return _letterbox.ShowBottomText("You defeated the human eye specimen R0-21.").AsCoroutine();
        yield return _letterbox.ShowCursor(true).AsCoroutine();

        yield return _letterbox.ShowBottomText("Its corruption clings unto you...").AsCoroutine();
        yield return _letterbox.ShowCursor(true).AsCoroutine();

        // change to human eye
        yield return _player.ChangeToHumanEyes().AsCoroutine();

        yield return _letterbox.ShowBottomText("You now have the curse of the flesh.").AsCoroutine();
        yield return _letterbox.ShowCursor(true).AsCoroutine();

        yield return _letterbox.ShowBottomText("You can't help but wonder... what might have changed?").AsCoroutine();
        yield return _letterbox.ShowCursor(true).AsCoroutine();

        yield return _letterbox.HideLetterbox().AsCoroutine();

        Trigger.Triggered = true;
        Trigger2.Triggered = true;
    }

    private void SpawnExplosion()
    {
        if (_explosion.TryGetFromPool(out var effect))
        {
            effect.Component.transform.position = Enemy.transform.position + (Vector3) Random.insideUnitCircle * ExplosionArea;
        }

        if (_explosion.TryGetFromPool(out var effect2))
        {
            effect2.Component.transform.position = Enemy.transform.position + (Vector3)Random.insideUnitCircle * ExplosionArea;
        }

        if (_explosion.TryGetFromPool(out var effect3))
        {
            effect3.Component.transform.position = Enemy.transform.position + (Vector3)Random.insideUnitCircle * ExplosionArea;
        }
    }
}
