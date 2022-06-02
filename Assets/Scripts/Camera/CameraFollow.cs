using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Objects;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;
    private Camera _camera;
    private Player _player;
    private RoomManager _roomManager;
    private ITime _uiTimer;

    protected void Awake()
    {
        _camera = GetComponent<Camera>();
        _player = Player.Instance();
        _roomManager = RoomManager.Instance();
        _uiTimer = DefaultUITimer.GetTimer();
    }

    private void OnEnable()
    {
        MachineryRef.Machinery.AddBasicMachine(Follow());
    }

    private IEnumerable<IEnumerable<Action>> Follow()
    {
        var currentRoom = _roomManager.CurrentRoom.Value;
        while (isActiveAndEnabled)
        {
            if (_roomManager.CurrentRoom.Value != currentRoom)
            {
                yield return TimeYields.WaitSeconds(_uiTimer, 0.5f);
                currentRoom = _roomManager.CurrentRoom.Value;
            }

            if (_roomManager.CurrentRoom.Value.RoomSize.x > 1)
            {
                var newPos = _player.transform.position;

                _camera.transform.position = new Vector3(
                    Mathf.Clamp(newPos.x, _roomManager.CurrentRoom.Value.RoomX * 15,
                        (_roomManager.CurrentRoom.Value.RoomX + _roomManager.CurrentRoom.Value.RoomSize.x-1) * 15),
                        _camera.transform.position.y, _camera.transform.position.z);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
