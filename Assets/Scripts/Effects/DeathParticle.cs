using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Pooling;
using UnityEngine;

public class DeathParticle : EffectPoolable
{
    public Vector2 Direction;
    public float Speed;
    public float DurationInSeconds;

    private ITime _uiTimer;

    private void Awake()
    {
        _uiTimer = DefaultUITimer.GetTimer();
    }

    public override void OnActivation()
    {
        BasicMachineryObject.Machinery.AddBasicMachine(RunEffect());
    }

    private IEnumerable<IEnumerable<Action>> RunEffect()
    {
        yield return TimeYields.WaitSeconds(_uiTimer, DurationInSeconds, fn =>
        {
            transform.position += (Vector3) ((float) _uiTimer.UpdatedTimeInMilliseconds * 0.001f * Speed * Direction);
        });

        IsEffectOver = true;
    }

    public override bool IsEffectOver { get; protected set; }
}
