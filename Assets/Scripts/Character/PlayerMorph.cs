using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using UnityEngine;

public class PlayerMorph : SceneObject<PlayerMorph>
{
    public SpriteRenderer RobotHead;
    public SpriteRenderer HumanHead;

    public AudioSource TransformAudio;

    private ITime _uiTimer;
    private float _originalLuminance;

    private void Awake()
    {
        _uiTimer = DefaultUITimer.GetTimer();

        _originalLuminance = HumanHead.material.GetFloat("_Luminance");
    }

    public void SetHeadForm(ScriptableForm form)
    {
        RobotHead.gameObject.SetActive(form.Form == ScriptableForm.CharacterForm.Robot);
        HumanHead.gameObject.SetActive(form.Form == ScriptableForm.CharacterForm.Human);
    }

    public IEnumerable<IEnumerable<Action>> MorphHeadToHuman()
    {
        yield return new LerpBuilder(f => RobotHead.material.SetFloat("_Luminance", f),
                () => RobotHead.material.GetFloat("_Luminance"))
            .SetTarget(1)
            .Over(1f)
            .Easing(EasingYields.EasingFunction.SineEaseIn)
            .UsingTimer(_uiTimer)
            .Build();
        
        HumanHead.material.SetFloat("_Luminance",1f);
        HumanHead.gameObject.SetActive(true);
        RobotHead.gameObject.SetActive(false);

        TransformAudio.Play();

        yield return new LerpBuilder(f => HumanHead.material.SetFloat("_Luminance", f),
                () => HumanHead.material.GetFloat("_Luminance"))
            .SetTarget(_originalLuminance)
            .Over(1f)
            .Easing(EasingYields.EasingFunction.SineEaseIn)
            .UsingTimer(_uiTimer)
            .Build();

    }
}

