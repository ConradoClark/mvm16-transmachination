using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Player : SceneObject<Player>
{
    public LayerMask LayerMask;
    public Collider2D MainCollider;
    public LichtPlatformerMoveController MoveController;
    public LichtPlatformerJumpController JumpController;
    public CharacterDashController DashController;
    public LichtMovementController BlasterController;
    public LichtPhysicsObject PhysicsObject;
    public float GettingHitDurationInSeconds;
    public float InvincibilityDurationInSeconds;
    public PlayerStats Stats;
    public ScriptableFormComposition Form;
    public Transform AnimTransform;

    private PlayerMorph _morph;
    private void Awake()
    {
        _morph = PlayerMorph.Instance();
        _morph.SetHeadForm(Form.Eyes);
    }

    public void UpdateForm()
    {
        if (_morph!=null) _morph.SetHeadForm(Form.Eyes);
    }

    public void Hide()
    {
        AnimTransform.gameObject.SetActive(false);
    }

    public void Show()
    {
        AnimTransform.gameObject.SetActive(true);
    }

    public void BlockAllMovement(MonoBehaviour source)
    {
        MoveController.BlockMovement(source);
        JumpController.BlockMovement(source);
        DashController.BlockMovement(source);
        BlasterController.BlockMovement(source);
    }

    public void UnblockAllMovement(MonoBehaviour source)
    {
        MoveController.UnblockMovement(source);
        JumpController.UnblockMovement(source);
        DashController.UnblockMovement(source);
        BlasterController.UnblockMovement(source);
    }

    public IEnumerable<IEnumerable<Action>> ChangeToHumanEyes()
    {
        yield return _morph.MorphHeadToHuman().AsCoroutine();
        Form.Eyes.Form = ScriptableForm.CharacterForm.Human;
    }
}
