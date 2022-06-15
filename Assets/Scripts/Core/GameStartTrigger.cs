using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameStartTrigger : RoomObject
{
    public Collider2D Collider;
    public ScriptableTriggerWatcher Trigger;
    public LayerMask CollisionLayerMask;

    private bool _hooked;
    private Collider2D[] _results;
    private Letterbox _letterbox;
    private Player _player;

    public override bool PerformReset()
    {
        if (_hooked) return false;
        _hooked = true;

        Collider.enabled = !Trigger.Trigger.Triggered;

        if (!Trigger.Trigger.Triggered)
        {
            DefaultMachinery.AddBasicMachine(HandleCollision());

            Trigger.OnTriggerChanged += Trigger_OnTriggerChanged;
        }

        return true;
    }

    public override void Initialize()
    {
        _results = new Collider2D[1];
        _letterbox = Letterbox.Instance();
        _player = Player.Instance();
    }

    public override bool Activate()
    {
        return true;
    }

    private IEnumerable<IEnumerable<Action>> HandleCollision()
    {
        var filter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = CollisionLayerMask,
        };

        while (Collider.OverlapCollider(filter, _results) <= 0)
        {
            if (!_hooked || !isActiveAndEnabled) yield break;
            yield return TimeYields.WaitMilliseconds(GameTimer, 30);
        }

        Trigger.Trigger.Triggered = true;
        yield return _letterbox.ShowLetterbox().AsCoroutine();

        yield return _letterbox.ShowBottomText("ASC-01 - Autonomous Scouting Cybernetical. Initializing...")
            .AsCoroutine();

        yield return _letterbox.ShowCursor(true).AsCoroutine();

        yield return _letterbox.ShowBottomText("Self-scan complete... 100% Robot. Analyzing area...")
            .AsCoroutine();

        yield return _letterbox.ShowCursor(true).AsCoroutine();

        yield return _letterbox.ShowBottomText("Area shows traces of rogue machinery and unscanned biological subjects. Initiating scout mode.")
            .AsCoroutine();

        yield return _letterbox.ShowCursor(true).AsCoroutine();

        yield return _letterbox.HideLetterbox().AsCoroutine();
    }

    private void Trigger_OnTriggerChanged(bool obj)
    {
        if (obj)
        {
            Collider.enabled = false;
        }
    }

    public override bool Deactivate()
    {
        Trigger.OnTriggerChanged -= Trigger_OnTriggerChanged;
        _hooked = false;
        return base.Deactivate();
    }
}
