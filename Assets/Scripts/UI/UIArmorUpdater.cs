using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Builders;
using TMPro;
using UnityEngine;

public class UIArmorUpdater : MonoBehaviour
{
    public TMP_Text TextComponent;
    public SpriteRenderer ArmorCounter;
    public SpriteRenderer ArmorCounterEffect;
    public float ArmorCounterTick;
    public int HealthPerTick;

    private ITime _uiTimer;
    private BasicMachinery<object> _defaultMachinery;
    private Player _player;

    private void Awake()
    {
        _uiTimer = DefaultUITimer.GetTimer();
        _defaultMachinery = DefaultMachinery.GetDefaultMachinery();
        _player = Player.Instance();

        ArmorCounter.size = new Vector2(GetSizeValue(_player.Stats.CurrentHitPoints), ArmorCounter.size.y);
        ArmorCounterEffect.size = new Vector2(GetSizeValue(_player.Stats.CurrentHitPoints), ArmorCounterEffect.size.y);
        TextComponent.text = _player.Stats.CurrentHitPoints.ToString();
    }

    private void OnEnable()
    {
        this.ObserveEvent<PlayerStats.StatChangeEvent, PlayerStats.StatChangeEventArgs>(PlayerStats.StatChangeEvent.OnHitPointsChanged, OnHitPointsChanged);
    }

    private void OnDisable()
    {
        this.StopObservingEvent<PlayerStats.StatChangeEvent, PlayerStats.StatChangeEventArgs>(PlayerStats.StatChangeEvent.OnHitPointsChanged, OnHitPointsChanged);
    }

    private float GetSizeValue(float hitPoints)
    {
        return hitPoints * ArmorCounterTick / HealthPerTick;
    }

    private void OnHitPointsChanged(PlayerStats.StatChangeEventArgs obj)
    {
        ArmorCounter.size = new Vector2(GetSizeValue(obj.CurrentValue), ArmorCounter.size.y);
        TextComponent.text = obj.CurrentValue.ToString();
        _defaultMachinery.AddBasicMachine(HitEffect(obj.CurrentValue));
    }
    private IEnumerable<IEnumerable<Action>> HitEffect(int targetValue)
    {
        yield return new LerpBuilder(
                f => ArmorCounterEffect.size = new Vector2(f,
                    ArmorCounterEffect.size.y),
                () => ArmorCounterEffect.size.x)
            .SetTarget(GetSizeValue(targetValue))
            .Over(0.75f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(_uiTimer)
            .Build();
    }
}