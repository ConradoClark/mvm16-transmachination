using Licht.Impl.Events;
using UnityEngine;

public class OnBlasterSfx : MonoBehaviour
{
    public RandomPitchAudio AudioSource;
    public RandomPitchAudio ImpactAudioSource;

    private Player _player;
    private void Awake()
    {
        _player = Player.Instance();
    }

    private void OnEnable()
    {
        this.ObserveEvent<WeaponEvents, WeaponEventArgs>(WeaponEvents.OnShoot, OnEvent);
        this.ObserveEvent<WeaponEvents, WeaponEventArgs>(WeaponEvents.OnImpact, OnImpact);
    }

    private void OnEvent(WeaponEventArgs args)
    {
        AudioSource.Play();
    }

    private void OnImpact(WeaponEventArgs args)
    {
        if (args.Source != _player.PhysicsObject) return;
        ImpactAudioSource.Play();
    }

    private void OnDisable()
    {
        this.StopObservingEvent<WeaponEvents, WeaponEventArgs>(WeaponEvents.OnShoot, OnEvent);
        this.StopObservingEvent<WeaponEvents, WeaponEventArgs>(WeaponEvents.OnImpact, OnImpact);
    }
}
