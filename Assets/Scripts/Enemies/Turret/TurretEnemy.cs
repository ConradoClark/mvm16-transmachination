using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class TurretEnemy : RoomObject
{
    private Enemy _enemy;
    private Vector3 _originalPosition;
    private GameObject _playerObject;
    public float Direction;
    public float ProjectileSpeed;
    public string Projectile;

    public override void Initialize()
    {
        _enemy = GetComponent<Enemy>();
        _originalPosition = transform.position;
        _playerObject = _playerObject == null ? GameObject.FindGameObjectWithTag("Player") : _playerObject;
    }

    public override bool Activate()
    {
        return true;
    }

    public override bool PerformReset()
    {
        transform.position = _originalPosition;
        _enemy.PerformReset();
        return true;
    }
}
