using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Builders;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class UIEnergyUpdater : MonoBehaviour
{
    public SpriteRenderer EnergyCounter;
    public SpriteRenderer EnergyCounterEffect;
    public float EnergyCounterTick;
    public int EnergyPerTick;

    private ITime _uiTimer;
    private BasicMachinery<object> _defaultMachinery;
    private Player _player;
    private bool _flickering;

    private void Awake()
    {
        _uiTimer = DefaultUITimer.GetTimer();
        _defaultMachinery = DefaultMachinery.GetDefaultMachinery();
        _player = Player.Instance();

        EnergyCounter.size = new Vector2(GetSizeValue(_player.Stats.CurrentEnergy), EnergyCounter.size.y);
        EnergyCounterEffect.size = new Vector2(GetSizeValue(_player.Stats.CurrentEnergy), EnergyCounterEffect.size.y);
    }

    private void OnEnable()
    {
        this.ObserveEvent<PlayerStats.StatChangeEvent, PlayerStats.StatChangeEventArgs>(PlayerStats.StatChangeEvent.OnEnergyChanged, OnEnergyChanged);
        this.ObserveEvent<PlayerStats.StatChangeEvent, PlayerStats.StatChangeEventArgs>(PlayerStats.StatChangeEvent.OnEnergyFailedConsumption, OnEnergyFailedConsumption);

        _defaultMachinery.AddBasicMachine(FollowValue());
    }

    private void OnEnergyFailedConsumption(PlayerStats.StatChangeEventArgs obj)
    {
        _defaultMachinery.AddBasicMachine(FlickerBar());
    }

    private IEnumerable<IEnumerable<Action>> FlickerBar()
    {
        if (_flickering) yield break;
        _flickering = true;

        for (var i = 0; i < 3; i++)
        {
            EnergyCounter.enabled = EnergyCounterEffect.enabled = !EnergyCounter.enabled;
            yield return TimeYields.WaitMilliseconds(_uiTimer, 100);
            if (!_flickering) break;
        }

        EnergyCounter.enabled = EnergyCounterEffect.enabled = true;

        _flickering = false;
    }

    private IEnumerable<IEnumerable<Action>> FollowValue()
    {
        while (isActiveAndEnabled)
        {
            var move = GetSizeValue(_player.Stats.CurrentEnergy) - EnergyCounterEffect.size.x;
            var sign = Mathf.Sign(move);
            var magnitude = Mathf.Clamp(Mathf.Abs(move), 0, 0.01f);

            EnergyCounterEffect.size = new Vector2(EnergyCounterEffect.size.x + magnitude * sign, EnergyCounterEffect.size.y);
            yield return TimeYields.WaitMilliseconds(_uiTimer, 10);
        }
    }

    private void OnDisable()
    {
        this.StopObservingEvent<PlayerStats.StatChangeEvent, PlayerStats.StatChangeEventArgs>(PlayerStats.StatChangeEvent.OnEnergyChanged, OnEnergyChanged);
    }

    private float GetSizeValue(int energy)
    {
        return energy * EnergyCounterTick / EnergyPerTick;
    }

    private void OnEnergyChanged(PlayerStats.StatChangeEventArgs obj)
    {
        EnergyCounter.size = new Vector2(GetSizeValue(obj.CurrentValue), EnergyCounter.size.y);
    }
}