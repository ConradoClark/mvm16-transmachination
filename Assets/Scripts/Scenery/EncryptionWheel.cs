using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class EncryptionWheel : BaseObject
{
    public SpriteRenderer Wheel;
    public SpriteRenderer Quadrant;
    public SpriteRenderer Cursor;
    public Collider2D QuadrantCollider;
    public Collider2D CursorCollider;
    public LayerMask CollisionLayerMask;
    public Animator CursorAnimator;

    public Color SuccessColor;
    public Color FailureColor;

    public bool HitOnQuadrant { get; private set; }
    public bool Hidden { get; private set; }

    private bool _animating;
    private const string MaterialOpacity = "_Opacity";
    private const string MaterialLuminance = "_Luminance";
    private Collider2D[] _results;
    private Color _transparentColor;
    private Player _player;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _results = new Collider2D[1];
        _transparentColor = new Color(0, 0, 0, 0);
        _player = Player.Instance();
    }

    private void RandomizeQuadrant()
    {
        var rng = Random.Range(0, 8);

        Quadrant.transform.rotation = Quaternion.identity;
        Quadrant.transform.Rotate(Vector3.forward, rng * 45);
        Quadrant.enabled = QuadrantCollider.enabled = _player.Form.Eyes.Form == ScriptableForm.CharacterForm.Robot;
    }

    public void HideInstantly()
    {
        Wheel.material.SetFloat(MaterialOpacity, 0);
        Quadrant.enabled = false;
        Cursor.material.SetFloat(MaterialOpacity, 0);
        Hidden = true;
    }

    private bool IsCursorOnQuadrant()
    {
        var filter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = CollisionLayerMask
        };

        return QuadrantCollider.OverlapCollider(filter, _results) > 0;
    }

    public IEnumerable<IEnumerable<Action>> Hit()
    {
        if (_animating)
        {
            _animating = false;
            yield return TimeYields.WaitOneFrameX;
        }

        _animating = true;

        var sprites = new[] { Wheel, Quadrant};

        HitOnQuadrant = IsCursorOnQuadrant();
        Cursor.material.SetColor("_Colorize", HitOnQuadrant ? SuccessColor : FailureColor);
        CursorAnimator.speed = 0;

         var flash = sprites.Aggregate(
            Enumerable.Empty<Action>(), (cur, spr) => cur.Combine(
                new LerpBuilder(f => spr.material.SetFloat(MaterialLuminance, f), () => spr.material.GetFloat(MaterialLuminance))
                    .SetTarget(1f)
                    .Over(0.25f)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .BreakIf(() => !_animating)
                    .UsingTimer(GameTimer)
                    .Build()));

        var flashBack = sprites.Aggregate(
            Enumerable.Empty<Action>(), (cur, spr) => cur.Combine(
                new LerpBuilder(f => spr.material.SetFloat(MaterialLuminance, f), () => spr.material.GetFloat(MaterialLuminance))
                    .SetTarget(0f)
                    .Over(0.25f)
                    .Easing(EasingYields.EasingFunction.CubicEaseIn)
                    .BreakIf(() => !_animating)
                    .UsingTimer(GameTimer)
                    .Build()));

        yield return flash.Then(flashBack);

        if (!HitOnQuadrant)
        {
            Cursor.material.SetColor("_Colorize", _transparentColor);
            CursorAnimator.speed = 1;
        }
        _animating = false;
    }

    public IEnumerable<IEnumerable<Action>> Hide()
    {
        Hidden = true;
        if (_animating)
        {
            _animating = false;
            yield return TimeYields.WaitOneFrameX;
        }

        _animating = true;

        var sprites = new[] { Wheel, Quadrant, Cursor };

        var fadeOut = sprites.Aggregate(
            Enumerable.Empty<Action>(), (cur, spr) => cur.Combine(
                    new LerpBuilder(f => spr.material.SetFloat(MaterialOpacity, f), () => spr.material.GetFloat(MaterialOpacity))
                .SetTarget(0f)
                .Over(0.25f)
                .Easing(EasingYields.EasingFunction.CubicEaseOut)
                .BreakIf(() => !_animating)
                .UsingTimer(GameTimer)
                .Build()));

        yield return fadeOut;
        Quadrant.enabled = false;
        _animating = false;
    }

    public IEnumerable<IEnumerable<Action>> Show()
    {
        Hidden = false;
        if (_animating)
        {
            _animating = false;
            yield return TimeYields.WaitOneFrameX;
        }

        _animating = true;

        var sprites = new[] { Wheel, Quadrant, Cursor };

        Cursor.material.SetColor("_Colorize", _transparentColor);

        RandomizeQuadrant();
        HitOnQuadrant = false;
        CursorAnimator.speed = 1f;

        var fadeIn = sprites.Aggregate(
            Enumerable.Empty<Action>(), (cur, spr) => cur.Combine(
                new LerpBuilder(f => spr.material.SetFloat(MaterialOpacity, f), () => spr.material.GetFloat(MaterialOpacity))
            .SetTarget(1f)
            .Over(0.25f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .BreakIf(() => !_animating)
            .UsingTimer(GameTimer)
            .Build()));

        yield return fadeIn;
        _animating = false;
    }
}
