using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;
using UnityEngine;

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