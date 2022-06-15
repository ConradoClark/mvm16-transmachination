using Licht.Unity.Physics;

public enum WeaponEvents
{
    OnShoot,
    OnImpact
}

public class WeaponEventArgs
{
    public LichtPhysicsObject Source;
    public Projectile Projectile;
}