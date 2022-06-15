using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Letterbox : SceneObject<Letterbox>
{
    public SpriteRenderer TopSprite;
    public SpriteRenderer BottomSprite;
    public Vector3 TopSpawnPosition;
    public Vector3 BottomSpawnPosition;

    public float CursorFrequencyInMs;
    public float TextFrequencyInMs;

    public TMP_Text BottomTextCaption;
    public TMP_Text BottomText;
    public SpriteRenderer Cursor;

    public ScriptableInputAction AcceptAction;

    private Vector3 _bottomSpriteOriginalPosition;
    private Vector3 _topSpriteOriginalPosition;
    private Color _originalColor;
    private ITime _uiTimer;
    private ITime _gameTimer;
    private Player _player;
    private PlayerInput _playerInput;
    private bool _showCursor;

    private void Awake()
    {
        _uiTimer = DefaultUITimer.GetTimer();
        _gameTimer = DefaultGameTimer.GetTimer();
        _bottomSpriteOriginalPosition = BottomSprite.transform.position;
        _topSpriteOriginalPosition = TopSprite.transform.position;
        TopSprite.transform.position = TopSpawnPosition;
        BottomSprite.transform.position = BottomSpawnPosition;

        BottomText.text = BottomTextCaption.text = "";

        _originalColor = BottomSprite.color;
        _player = Player.Instance();
        _playerInput = PlayerInput.GetPlayerByIndex(0);
    }

    public IEnumerable<IEnumerable<Action>> ShowLetterbox()
    {
        _playerInput.SwitchCurrentActionMap("UI");
        BottomText.text = BottomTextCaption.text = "";
        BottomSprite.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0);

        _player.BlockAllMovement(this);

        var showBottom = BottomSprite.transform.GetAccessor()
            .Position
            .Y
            .SetTarget(_bottomSpriteOriginalPosition.y)
            .Over(2f)
            .UsingTimer(_uiTimer)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .Build();

        var showTop = TopSprite.transform.GetAccessor()
            .Position
            .Y
            .SetTarget(_topSpriteOriginalPosition.y)
            .Over(2f)
            .UsingTimer(_uiTimer)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .Build();

        var fadeIn = BottomSprite.GetAccessor()
            .Color
            .A
            .SetTarget(1f)
            .Over(2f)
            .UsingTimer(_uiTimer)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .Build();

        yield return showBottom.Combine(fadeIn).Combine(showTop);
    }

    public IEnumerable<IEnumerable<Action>> ShowBottomCaption(string text)
    {
        var acceptAction = _playerInput.actions[AcceptAction.ActionName];
        yield return TimeYields.WaitOneFrameX;

        var index = 0;
        while (BottomTextCaption.text != text)
        {
            if (acceptAction.WasPerformedThisFrame())
            {
                BottomTextCaption.text = text;
                continue;
            }

            BottomTextCaption.text = text.Substring(0, index + 1);
            index++;

            if (BottomTextCaption.text.Last() == '<')
            {
                do
                {
                    BottomTextCaption.text = text.Substring(0, index + 1);
                    index++;
                } while (BottomTextCaption.text.Last() != '>' && index < text.Length);
            }

            yield return TimeYields.WaitMilliseconds(_uiTimer, TextFrequencyInMs,
                breakCondition: () => acceptAction.WasPerformedThisFrame());
        }
    }

    public IEnumerable<IEnumerable<Action>> ShowBottomText(string text)
    {
        var acceptAction = _playerInput.actions[AcceptAction.ActionName];
        yield return TimeYields.WaitOneFrameX;

        var index = 0;
        while (BottomText.text != text)
        {
            if (acceptAction.WasPerformedThisFrame())
            {
                BottomText.text = text;
                continue;
            }

            BottomText.text = text.Substring(0, index + 1);
            index++;
            yield return TimeYields.WaitMilliseconds(_uiTimer, TextFrequencyInMs,
                breakCondition: () => acceptAction.WasPerformedThisFrame());
        }
    }

    public IEnumerable<IEnumerable<Action>> ShowCursor(bool allowHide)
    {
        var acceptAction = _playerInput.actions[AcceptAction.ActionName];
        yield return TimeYields.WaitOneFrameX;
        _showCursor = true;
        while (_showCursor)
        {
            Cursor.enabled = !Cursor.enabled;
            yield return TimeYields.WaitMilliseconds(_uiTimer, CursorFrequencyInMs,
                breakCondition: () => allowHide && acceptAction.WasPerformedThisFrame());

            if (allowHide && acceptAction.WasPerformedThisFrame())
            {
                _showCursor = false;
            }
        }
    }

    public void HideCursor()
    {
        _showCursor = false;
    }

    public IEnumerable<IEnumerable<Action>> HideLetterbox()
    {
        _playerInput.SwitchCurrentActionMap("Character");
        BottomText.text = BottomTextCaption.text = "";

        _player.UnblockAllMovement(this);

        var hideBottom = BottomSprite.transform.GetAccessor()
            .Position
            .Y
            .SetTarget(BottomSpawnPosition.y)
            .Over(2f)
            .UsingTimer(_uiTimer)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .Build();

        var hideTop = TopSprite.transform.GetAccessor()
            .Position
            .Y
            .SetTarget(TopSpawnPosition.y)
            .Over(2f)
            .UsingTimer(_uiTimer)
            .Easing(EasingYields.EasingFunction.SineEaseInOut)
            .Build();

        var fadeOut = BottomSprite.GetAccessor()
           .Color
           .A
           .SetTarget(0f)
           .Over(2f)
           .UsingTimer(_uiTimer)
           .Easing(EasingYields.EasingFunction.SineEaseInOut)
           .Build();

        yield return hideBottom.Combine(fadeOut).Combine(hideTop);
    }

}