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
        OnHitPointsChanged
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

    public void TakeDamage(int damage)
    {
        CurrentHitPoints -= damage;
        _eventPublisher.PublishEvent(StatChangeEvent.OnHitPointsChanged, new StatChangeEventArgs { CurrentValue = CurrentHitPoints });
    }
}
