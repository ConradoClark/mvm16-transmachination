using Licht.Impl.Events;
using Licht.Interfaces.Events;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int MaxHitPoints;
    public int CurrentHitPoints { get; private set; }

    private IEventPublisher<StatChangeEvent, StatChangeEventArgs> _eventPublisher;

    public enum StatChangeEvent
    {
        OnHitPointsChanged,
        OnDeath
    }

    public class StatChangeEventArgs
    {
        public int CurrentValue;
    }

    private void Awake()
    {
        CurrentHitPoints = MaxHitPoints;
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
        _eventPublisher.PublishEvent(StatChangeEvent.OnHitPointsChanged, new StatChangeEventArgs { CurrentValue = CurrentHitPoints });
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
        }
    }
}
