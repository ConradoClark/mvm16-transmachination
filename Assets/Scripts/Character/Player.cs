using System.Collections;
using System.Collections.Generic;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Player : SceneObject<Player>
{
    public Collider2D MainCollider;
    public LichtPlatformerMoveController MoveController;
    public LichtPlatformerJumpController JumpController;
    public CharacterDashController DashController;
    public LichtMovementController BlasterController;
    public LichtPhysicsObject PhysicsObject;
    public float GettingHitDurationInSeconds;
    public float InvincibilityDurationInSeconds;
    public PlayerStats Stats;
}
