using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using UnityEngine;

public class OnDashSfx : MonoBehaviour
{
    public RandomPitchAudio AudioSource;
    private void OnEnable()
    {
        this.ObserveEvent(CharacterDashController.DashEvents.OnDashStart, OnEvent);
    }

    private void OnEvent()
    {
        AudioSource.Play();
    }

    private void OnDisable()
    {
        this.StopObservingEvent(CharacterDashController.DashEvents.OnDashStart, OnEvent);
    }
}
