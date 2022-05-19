using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MoveCameraToNextArea : MonoBehaviour
{
    private Camera _camera;
    private BasicMachinery<object> _defaultMachinery;
    private void OnEnable()
    {
        _camera = _camera != null ? _camera : GetComponent<Camera>();
        this.ObserveEvent<RoomExit.RoomExitEvents, RoomExit.RoomExitEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnExit);

        _defaultMachinery ??= DefaultMachinery.GetDefaultMachinery();
    }

    private void OnDisable()
    {
        this.StopObservingEvent<RoomExit.RoomExitEvents, RoomExit.RoomExitEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnExit);
    }

    private void OnExit(RoomExit.RoomExitEventArgs obj)
    {
        _camera.transform.position = new Vector3(obj.Source.ToRoom.transform.position.x,
            obj.Source.ToRoom.transform.position.y, _camera.transform.position.z);
    }
}
