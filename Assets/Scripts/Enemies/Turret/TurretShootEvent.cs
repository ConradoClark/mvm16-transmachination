using Licht.Unity.Pooling;
using UnityEngine;

public class TurretShootEvent : BaseObject
{
    public TurretEnemy TurretEnemy;
    public Vector2 EffectOffset;
    public Vector2 ProjectileOffset;
    public string TurretShootEffectName;

    private EffectsManager _effectsManager;
    private ProjectilesManager _projectilesManager;
    private PrefabPool _turretShootEffect;
    private ProjectilePool _turretProjectilePool;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _effectsManager = EffectsManager.Instance();
        _projectilesManager = ProjectilesManager.Instance();
        _turretShootEffect = _effectsManager.GetEffect(TurretShootEffectName);
        _turretProjectilePool = _projectilesManager.GetProjectile(TurretEnemy.Projectile);
    }

    public void OnShoot()
    {
        if (_turretShootEffect.TryGetFromPool(out var obj))
        {
            obj.Component.transform.position = new Vector3(transform.position.x + EffectOffset.x * TurretEnemy.Direction,
                transform.position.y + EffectOffset.y);
        }

        if (_turretProjectilePool.TryGetFromPool(out var proj))
        {
            proj.Component.transform.position = transform.position + new Vector3(ProjectileOffset.x * TurretEnemy.Direction, ProjectileOffset.y);
            proj.Direction = new Vector2(TurretEnemy.Direction, 0);
            proj.Speed = TurretEnemy.ProjectileSpeed;
        }
    }
}
