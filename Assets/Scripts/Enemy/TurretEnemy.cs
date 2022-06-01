using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class TurretEnemy : RoomObject
{
    public string TurretEffect;
    private Enemy _enemy;
    private Vector3 _originalPosition;
    private EffectsManager _effectsManager;
    private PrefabPool _turretShootEffect;
    private GameObject _playerObject;

    public override void Initialize()
    {
        _enemy = GetComponent<Enemy>();
        _originalPosition = transform.position;
        _effectsManager = EffectsManager.GetInstance();
        _turretShootEffect = _effectsManager.GetEffect(TurretEffect);
        _playerObject = _playerObject == null ? GameObject.FindGameObjectWithTag("Player") : _playerObject;
    }

    public override bool Activate()
    {
        DefaultMachinery.AddBasicMachine(HandleTurret());
        return true;
    }

    public override bool PerformReset()
    {
        transform.position = _originalPosition;
        _enemy.PerformReset();
        return true;
    }

    private IEnumerable<IEnumerable<Action>> HandleTurret()
    {
        while (IsActive)
        {
            if (Vector2.Distance(transform.position, _playerObject.transform.position) < 1f)
            {
                Debug.Log("player is close");
                yield return TimeYields.WaitSeconds(GameTimer, 1);
            }
            
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
