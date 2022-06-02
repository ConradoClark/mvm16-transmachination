using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MoveCameraToNextArea : BaseObject
{
    private Camera _camera;
    private ITime _uiTimer;
    private void OnEnable()
    {
        _camera = _camera != null ? _camera : GetComponent<Camera>();
        _uiTimer ??= DefaultUITimer.GetTimer();
        this.ObserveEvent<RoomExit.RoomExitEvents, RoomExit.RoomExitEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnExit);
    }

    private void OnDisable()
    {
        this.StopObservingEvent<RoomExit.RoomExitEvents, RoomExit.RoomExitEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnExit);
    }

    private void OnExit(RoomExit.RoomExitEventArgs obj)
    {

        DefaultMachinery.AddBasicMachine(MoveCamera(obj));

        //_camera.transform.position = new Vector3(obj.Source.ToRoom.transform.position.x,
        //    obj.Source.ToRoom.transform.position.y, _camera.transform.position.z);
    }

    private IEnumerable<IEnumerable<Action>> MoveCamera(RoomExit.RoomExitEventArgs obj)
    {
        GameTimer.Multiplier = 0;

        yield return _camera.transform.GetAccessor()
            .Position
            .X
            .SetTarget(obj.Source.ToRoom.transform.position.x)
            .Over(0.4f)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .UsingTimer(_uiTimer)
            .Build();

        GameTimer.Multiplier = 1;
    }
}
