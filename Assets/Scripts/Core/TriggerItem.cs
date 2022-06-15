using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class TriggerItem : RoomObject
{
    public SpriteRenderer Sprite;
    public Collider2D Collider;
    public string Effect;
    public ScriptableTriggerWatcher Trigger;
    public LayerMask CollisionLayerMask;

    public AudioSource Pickup;

    public string ItemTitle;
    public string ItemDescription;

    private bool _hooked;
    private EffectsManager _effects;
    private PrefabPool _effectPool;
    private Collider2D[] _results;
    private Letterbox _letterbox;
    private Player _player;

    public override bool PerformReset()
    {
        if (_hooked) return false;
        _hooked = true;

        Sprite.enabled = !Trigger.Trigger.Triggered;
        Collider.enabled = !Trigger.Trigger.Triggered;

        if (!Trigger.Trigger.Triggered)
        {
            Sprite.enabled = Collider.enabled = true;
            DefaultMachinery.AddBasicMachine(SpawnParticles());
            DefaultMachinery.AddBasicMachine(HandleCollision());

            Trigger.OnTriggerChanged += Trigger_OnTriggerChanged;
        }

        return true;
    }

    public override void Initialize()
    {
        _effects = EffectsManager.Instance();
        _effectPool = _effects.GetEffect(Effect);
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

        Pickup.Play();
        Trigger.Trigger.Triggered = true;
        yield return _letterbox.ShowLetterbox().AsCoroutine();

        yield return _letterbox.ShowBottomCaption(ItemTitle).AsCoroutine();
        yield return _letterbox.ShowBottomText(ItemDescription).AsCoroutine();

        yield return _letterbox.ShowCursor(true).AsCoroutine();

        switch (Trigger.Trigger.Type)
        {
            case ScriptableTrigger.TriggerType.ArmorUp:
                _player.Stats.IncreaseMaxArmor();
                break;
            case ScriptableTrigger.TriggerType.EnergyUp:
                _player.Stats.IncreaseMaxEnergy();
                break;
            default: break;
        }

        yield return _letterbox.HideLetterbox().AsCoroutine();



    }

    private void Trigger_OnTriggerChanged(bool obj)
    {
        if (obj)
        {
            Sprite.enabled = Collider.enabled = false;
        }
    }

    public override bool Deactivate()
    {
        Trigger.OnTriggerChanged -= Trigger_OnTriggerChanged;
        _hooked = false;
        return base.Deactivate();
    }

    private IEnumerable<IEnumerable<Action>> SpawnParticles()
    {
        while (!Trigger.Trigger.Triggered)
        {
            if (_effectPool.TryGetFromPool(out var obj))
            {
                obj.Component.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.4f;
            }

            yield return TimeYields.WaitMilliseconds(GameTimer, 100);
        }
    }
}
