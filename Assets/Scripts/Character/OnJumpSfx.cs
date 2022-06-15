using Licht.Impl.Events;
using Licht.Unity.CharacterControllers;
using UnityEngine;

public class OnJumpSfx : MonoBehaviour
{
    public RandomPitchAudio AudioSource;
    private void OnEnable()
    {
        this.ObserveEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>
            (LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart, OnEvent);
    }

    private void OnEvent(LichtPlatformerJumpController.LichtPlatformerJumpEventArgs args)
    {
        AudioSource.Play();
    }

    private void OnDisable()
    {
        this.StopObservingEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>
            (LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart, OnEvent);
    }
}
