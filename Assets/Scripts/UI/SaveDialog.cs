using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveDialog : SceneObject<SaveDialog>
{
    public Transform Container;
    private ITime _uiTimer;
    private Player _player;
    private PlayerInput _playerInput;

    public KnownRoomsScriptable KnownRoomsRef;
    public SavePoint CurrentSave;

    [Serializable]
    public enum SaveDialogOption
    {
        Yes,
        No
    }

    public SaveDialogOption? DialogOption { get; set; }

    private void Awake()
    {
        _uiTimer = DefaultUITimer.GetTimer();
        _player = Player.Instance();
        _playerInput= PlayerInput.GetPlayerByIndex(0);
    }
    public IEnumerable<IEnumerable<Action>> ShowDialog(RoomDefinition room, TMP_Text gameSavedText)
    {
        Container.gameObject.SetActive(true);
        _player.PhysicsObject.enabled = false;
        _player.BlockAllMovement(this);
        _playerInput.SwitchCurrentActionMap("UI");
        DialogOption = null;

        while (DialogOption == null)
        {
            yield return TimeYields.WaitOneFrameX;
        }

        if (DialogOption == SaveDialogOption.Yes)
        {
            CurrentSave.Triggers = CurrentSave.AllTriggers.Triggers.Select(s =>
                new TriggerSettings
                {
                    Enabled = s.Triggered,
                    Trigger = s
                }).ToArray();

            CurrentSave.KnownRooms = KnownRoomsRef.KnownRooms;
            CurrentSave.Room = room;

            SavePoint.SaveFile(CurrentSave.Slot, CurrentSave);

            gameSavedText.enabled = true;
            for (var i = 0; i < 5; i++)
            {
                gameSavedText.enabled = !gameSavedText.enabled;
                yield return TimeYields.WaitMilliseconds(_uiTimer, 200);
            }

            gameSavedText.enabled = false;
        }

        _player.PhysicsObject.enabled = true;
        _player.UnblockAllMovement(this);
        _playerInput.SwitchCurrentActionMap("Character");
        Container.gameObject.SetActive(false);
    }

}

