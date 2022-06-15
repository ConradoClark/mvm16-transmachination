using Licht.Impl.Events;
using UnityEngine;

public class OnBlasterSfx : MonoBehaviour
{
    public RandomPitchAudio AudioSource;
    public RandomPitchAudio ImpactAudioSource;

    public RandomPitchAudio EyeBlasterAudioSource;
    public RandomPitchAudio EyeBlasterImpact;

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
        if (_player.Form.Eyes.Form == ScriptableForm.CharacterForm.Human)
        {
            EyeBlasterAudioSource.Play();
        }
        else AudioSource.Play();
    }

    private void OnImpact(WeaponEventArgs args)
    {
        if (args.Source != _player.PhysicsObject) return;
        if (_player.Form.Eyes.Form == ScriptableForm.CharacterForm.Human)
        {
            EyeBlasterImpact.Play();
        }
        else ImpactAudioSource.Play();
    }

    private void OnDisable()
    {
        this.StopObservingEvent<WeaponEvents, WeaponEventArgs>(WeaponEvents.OnShoot, OnEvent);
        this.StopObservingEvent<WeaponEvents, WeaponEventArgs>(WeaponEvents.OnImpact, OnImpact);
    }
}
