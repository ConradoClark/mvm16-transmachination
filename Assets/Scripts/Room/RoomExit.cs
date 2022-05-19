using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Memory;
using UnityEditor;
using UnityEngine;

public class RoomExit : MonoBehaviour
{
    public Vector2Int Direction;
    public Room FromRoom;
    public Room ToRoom;

    private Transform _player;
    private FrameVariables _frameVariables;
    private BasicMachinery<object> _defaultMachinery;
    private IEventPublisher<RoomExitEvents, RoomExitEventArgs> _eventPublisher;

    public RoomScriptable CurrentRoom;

    public enum RoomExitEvents
    {
        OnRoomExit
    }

    public class RoomExitEventArgs
    {
        public RoomExit Source;
    }

    private void OnEnable()
    {
        _eventPublisher = this.RegisterAsEventPublisher<RoomExitEvents, RoomExitEventArgs>();
        InitFrameVars();
        _player = _frameVariables.Get(new FrameVariableDefinition<Transform>("PlayerTransform",
            () => GameObject.FindGameObjectWithTag("Player").transform));

        _defaultMachinery ??= DefaultMachinery.GetDefaultMachinery();
        _defaultMachinery.AddBasicMachine(HandleExit());
    }

    private void OnDisable()
    {
        this.UnregisterAsEventPublisher<RoomExitEvents, RoomExitEventArgs>();
    }

    private void InitFrameVars()
    {
        if (_frameVariables != null) return;
        _frameVariables = FindObjectOfType<FrameVariables>();

        if (_frameVariables != null) return;
        var obj = new GameObject("frameVars");
        _frameVariables = obj.AddComponent<FrameVariables>();
    }

    private IEnumerable<IEnumerable<Action>> HandleExit()
    {
        while (isActiveAndEnabled)
        {
            var right = Direction == Vector2Int.right && _player.transform.position.x > transform.position.x &&
                        CurrentRoom.Value == FromRoom.RoomDefinition;

            var left = Direction == Vector2Int.left && _player.transform.position.x < transform.position.x &&
                       CurrentRoom.Value == FromRoom.RoomDefinition;

            if (right || left)
            {
                CurrentRoom.Value = ToRoom.RoomDefinition;
                FireRoomExitEvent();
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private void FireRoomExitEvent()
    {
        _eventPublisher.PublishEvent(RoomExitEvents.OnRoomExit, new RoomExitEventArgs { Source = this });
    }
}
