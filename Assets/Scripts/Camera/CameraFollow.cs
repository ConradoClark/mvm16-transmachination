using System;
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

    public bool IsXBlocked { get; set; }
    public bool IsYBlocked { get; set; }

    protected void Awake()
    {
        _camera = GetComponent<Camera>();
        _player = Player.Instance();
        _roomManager = RoomManager.Instance();
        _uiTimer = DefaultUITimer.GetTimer();
    }

    private void OnEnable()
    {
        _camera.transform.position = new Vector3(
            Mathf.Clamp(_player.transform.position.x, _roomManager.CurrentRoom.Value.RoomX * 15,
                (_roomManager.CurrentRoom.Value.RoomX + _roomManager.CurrentRoom.Value.RoomSize.x - 1) * 15),
            _roomManager.CurrentRoom.Value.RoomY * 9, _camera.transform.position.z);

        MachineryRef.Machinery.AddBasicMachine(Follow());
    }

    private IEnumerable<IEnumerable<Action>> Follow()
    {
        var currentRoom = _roomManager.CurrentRoom.Value;
        while (isActiveAndEnabled)
        {
            var newPos = _player.transform.position;
            if (!IsXBlocked && _roomManager.CurrentRoom.Value.RoomSize.x > 1)
            {
                _camera.transform.position = new Vector3(
                    Mathf.Clamp(newPos.x, _roomManager.CurrentRoom.Value.RoomX * 15,
                        (_roomManager.CurrentRoom.Value.RoomX + _roomManager.CurrentRoom.Value.RoomSize.x - 1) * 15),
                        _camera.transform.position.y, _camera.transform.position.z);
            }

            if (!IsYBlocked && _roomManager.CurrentRoom.Value.RoomSize.y > 1)
            {
                _camera.transform.position = new Vector3(
                    _camera.transform.position.x,
                    Mathf.Clamp(newPos.y, _roomManager.CurrentRoom.Value.RoomY * 9,
                        (_roomManager.CurrentRoom.Value.RoomY + _roomManager.CurrentRoom.Value.RoomSize.y - 1) * 9), _camera.transform.position.z);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
