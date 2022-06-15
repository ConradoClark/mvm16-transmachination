using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;

public class BossTrigger : RoomObject
{
    public ScriptableTrigger Trigger;
    public Collider2D Collider;
    public LayerMask CollisionLayerMask;

    public SpriteRenderer Silhouette;
    public Transform Boss;
    private Letterbox _letterbox;

    private Collider2D[] _results;
    private bool _hooked;

    public AudioSource BossMusic;

    public override bool PerformReset()
    {
        Boss.gameObject.SetActive(false);
        return true;
    }

    public override void Initialize()
    {
        _letterbox = Letterbox.Instance();
        _results = new Collider2D[1];
    }

    public override bool Activate()
    {
        if (!Trigger.Triggered && !_hooked)
        {
            _hooked = true;
            DefaultMachinery.AddBasicMachine(HandleTrigger());
        }
        return true;
    }

    public override bool Deactivate()
    {
        _hooked = false;
        return base.Deactivate();
    }

    private IEnumerable<IEnumerable<Action>> HandleTrigger()
    {
        var filter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = CollisionLayerMask,
        };

        while (Collider.OverlapCollider(filter, _results) <= 0)
        {
            if (!_hooked) yield break;
            yield return TimeYields.WaitMilliseconds(GameTimer, 30);
        }

        Silhouette.enabled = true;
        yield return _letterbox.ShowLetterbox().AsCoroutine();

        yield return _letterbox.ShowBottomText("As you reach the depths of the facility, you see a silhouette.").AsCoroutine();
        yield return _letterbox.ShowCursor(true).AsCoroutine();

        yield return _letterbox.ShowBottomText("A corrupted, pulsating eye, gazes upon you.").AsCoroutine();
        yield return _letterbox.ShowCursor(true).AsCoroutine();

        yield return _letterbox.ShowBottomText("Unknown unit approaching. Danger imminent.").AsCoroutine();
        yield return _letterbox.ShowCursor(true).AsCoroutine();

        BossMusic.Play();

        Silhouette.enabled = false;
        Boss.gameObject.SetActive(true);

        yield return _letterbox.HideLetterbox().AsCoroutine();
    }
}
