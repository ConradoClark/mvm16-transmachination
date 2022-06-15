using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SavePlatform : RoomObject
{
    public LayerMask CollisionLayerMask;
    public Collider2D[] Colliders;
    public float EffectFrequencyInMs;
    public Vector2 Offset;
    public Vector2 Size;
    public TMP_Text GameSavedText;

    private EffectsManager _effects;
    private PrefabPool _saveEffect;
    private Collider2D[] _results;
    private SaveDialog _saveDialog;
    
    public override bool PerformReset()
    {
        return true;
    }

    public override void Initialize()
    {
        _effects = EffectsManager.Instance();
        _saveEffect = _effects.GetEffect("SaveEffect");
        _results = new Collider2D[1];
        _saveDialog = SaveDialog.Instance();
    }

    public override bool Activate()
    {
        DefaultMachinery.AddBasicMachine(ShowParticles());
        DefaultMachinery.AddBasicMachine(HandleCollision());
        return true;
    }

    private IEnumerable<IEnumerable<Action>> HandleCollision()
    {
        var filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = CollisionLayerMask
        };

        while (isActiveAndEnabled)
        {
            if (Colliders.All(c=>c.OverlapCollider(filter, _results) > 0))
            {
                yield return _saveDialog.ShowDialog(CurrentRoom.Value, GameSavedText).AsCoroutine();
            }

            while (Colliders.All(c => c.OverlapCollider(filter, _results) > 0))
            {
                yield return TimeYields.WaitOneFrameX;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> ShowParticles()
    {
        while (isActiveAndEnabled)
        {
            if (_saveEffect.TryGetFromPool(out var obj))
            {
                var randomPos = (Vector3)Random.insideUnitCircle;
                obj.Component.transform.position = transform.position + (Vector3)Offset +
                                                   new Vector3(randomPos.x * Size.x, randomPos.y * Size.y);
            }
            yield return TimeYields.WaitMilliseconds(GameTimer, EffectFrequencyInMs);
        }
    }
}
