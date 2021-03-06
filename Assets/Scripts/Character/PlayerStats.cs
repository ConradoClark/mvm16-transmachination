using System.Linq;
using Licht.Impl.Events;
using Licht.Interfaces.Events;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int MaxHitPoints;
    public int CurrentHitPoints { get; private set; }

    public int MaxEnergy;
    public int CurrentEnergy { get; private set; }

    private IEventPublisher<StatChangeEvent, StatChangeEventArgs> _eventPublisher;

    public AudioSource Death;

    public AllTriggers AllTriggers;

    public enum StatChangeEvent
    {
        OnHitPointsChanged,
        OnEnergyChanged,
        OnEnergyFailedConsumption,
        OnDeath
    }

    public class StatChangeEventArgs
    {
        public int CurrentValue;
    }

    private void Awake()
    {
        CurrentHitPoints = MaxHitPoints += AllTriggers.Triggers.Count(t => t.Triggered && t.Type == ScriptableTrigger.TriggerType.ArmorUp) * 20;
        CurrentEnergy = MaxEnergy += AllTriggers.Triggers.Count(t => t.Triggered && t.Type == ScriptableTrigger.TriggerType.EnergyUp) * 100;
    }

    private void OnEnable()
    {
        _eventPublisher = this.RegisterAsEventPublisher<StatChangeEvent, StatChangeEventArgs>();
    }

    private void OnDisable()
    {
        this.UnregisterAsEventPublisher<StatChangeEvent, StatChangeEventArgs>();
    }

    public void ResetHitPoints()
    {
        CurrentHitPoints = MaxHitPoints;
        CurrentEnergy = MaxEnergy;
        _eventPublisher.PublishEvent(StatChangeEvent.OnHitPointsChanged, new StatChangeEventArgs { CurrentValue = CurrentHitPoints });
        _eventPublisher.PublishEvent(StatChangeEvent.OnEnergyChanged, new StatChangeEventArgs { CurrentValue = CurrentEnergy });
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHitPoints <= 0) return;

        CurrentHitPoints -= damage;
        if (CurrentHitPoints <= 0) CurrentHitPoints = 0;
        _eventPublisher.PublishEvent(StatChangeEvent.OnHitPointsChanged, new StatChangeEventArgs { CurrentValue = CurrentHitPoints });

        if (CurrentHitPoints == 0)
        {
            _eventPublisher.PublishEvent(StatChangeEvent.OnDeath, new StatChangeEventArgs());
            Death.Play();
        }
    }

    public void RestoreEnergy(int energy)
    {
        if (CurrentEnergy == MaxEnergy) return;

        CurrentEnergy += energy;
        if (CurrentEnergy > MaxEnergy) CurrentEnergy = MaxEnergy;
        _eventPublisher.PublishEvent(StatChangeEvent.OnEnergyChanged, new StatChangeEventArgs { CurrentValue = CurrentEnergy });
    }

    public bool ConsumeEnergy(int energy)
    {
        if (CurrentEnergy < energy)
        {
            _eventPublisher.PublishEvent(StatChangeEvent.OnEnergyFailedConsumption, new StatChangeEventArgs { CurrentValue = CurrentEnergy });
            return false;
        }

        CurrentEnergy -= energy;

        _eventPublisher.PublishEvent(StatChangeEvent.OnEnergyChanged, new StatChangeEventArgs { CurrentValue = CurrentEnergy });
        return true;
    }

    public void IncreaseMaxArmor()
    {
        MaxHitPoints += 20;
        CurrentHitPoints += 20;
        _eventPublisher.PublishEvent(StatChangeEvent.OnHitPointsChanged, new StatChangeEventArgs { CurrentValue = CurrentHitPoints });
    }

    public void IncreaseMaxEnergy()
    {
        MaxEnergy += 100;
        CurrentEnergy += 100;
        _eventPublisher.PublishEvent(StatChangeEvent.OnEnergyChanged, new StatChangeEventArgs { CurrentValue = CurrentEnergy });
    }
}
