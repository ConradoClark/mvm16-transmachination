using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class OptionsAudioKnob : MainMenuUIButton
{
    public AudioSource TestAudio;
    public AudioMixerGroup Mixer;
    public SpriteRenderer Gauge;

    public string VolumeParam;
    public float GaugeSize;

    public ScriptableInputAction UIHorizontal;

    public int SelectedLevel { get; private set; }

    private static readonly Dictionary<int, float> Levels = new Dictionary<int, float>
    {
        { 0, -80f },
        { 1, -30f },
        { 2, -25f },
        { 3, -18.7f },
        { 4, -11.2f },
        { 5, -9f },
        { 6, -4.9f },
        { 7, 0 },
        { 8, 4.5f },
        { 9, 8.3f },
    };

    private PlayerInput _playerInput;

    protected override void UnityAwake()
    {
        base.UnityAwake();

        _playerInput = PlayerInput.GetPlayerByIndex(0);

        SelectedLevel = 0;
        foreach (var level in Levels)
        {
            if (Mixer.audioMixer.GetFloat(VolumeParam, out var val))
            {
                if (val <= level.Value) break;
                SelectedLevel = level.Key;
            }
        }

        Gauge.size = new Vector2(GaugeSize * SelectedLevel , Gauge.size.y);
    }

    public override void OnInit()
    {
        base.OnInit();
        DefaultMachinery.AddBasicMachine(HandleKnob());
    }

    private IEnumerable<IEnumerable<Action>> HandleKnob()
    {
        var action = _playerInput.actions[UIHorizontal.ActionName];

        while (isActiveAndEnabled)
        {
            if (action.WasPerformedThisFrame() && Selected)
            {
                if (TestAudio!=null) TestAudio.Play();
                var sign = Mathf.RoundToInt(Mathf.Sign(action.ReadValue<float>()));
                SelectedLevel += sign;
                SelectedLevel = Mathf.Clamp(SelectedLevel, 0, 9);
                Gauge.size = new Vector2(GaugeSize * SelectedLevel, Gauge.size.y);
                Mixer.audioMixer.SetFloat(VolumeParam, Levels[SelectedLevel]);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    public override IEnumerable<IEnumerable<Action>> OnAction()
    {
        if (TestAudio != null) TestAudio.Play();

        yield break;
    }
}

