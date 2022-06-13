using Licht.Impl.Events;
using Licht.Unity.Objects;
public class DeathScreen : SceneObject<DeathScreen>
{
    public DeathEffects DeathEffects;
    private void OnEnable()
    {
        this.ObserveEvent<PlayerStats.StatChangeEvent, PlayerStats.StatChangeEventArgs>(PlayerStats.StatChangeEvent.OnDeath, OnEvent);
    }

    private void OnEvent(PlayerStats.StatChangeEventArgs obj)
    {
        ShowDeath();
    }

    private void OnDisable()
    {
        this.StopObservingEvent<PlayerStats.StatChangeEvent, PlayerStats.StatChangeEventArgs>(PlayerStats.StatChangeEvent.OnDeath, OnEvent);
    }

    public void ShowDeath()
    {
        DeathEffects.ShowDeath();
    }
}
