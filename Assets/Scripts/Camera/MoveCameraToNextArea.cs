using System;
using System.Collections.Generic;
using System.Linq;
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
    private Player _player;
    private CameraFollow _cameraFollow;

    protected override void UnityAwake()
    {
        base.UnityAwake();
        _player = Player.Instance();
        _cameraFollow = GetComponent<CameraFollow>();
    }

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
        _cameraFollow.IsXBlocked = true;
        DefaultMachinery.AddBasicMachine(MoveCamera(obj));

        //_camera.transform.position = new Vector3(obj.Source.ToRoom.transform.position.x,
        //    obj.Source.ToRoom.transform.position.y, _camera.transform.position.z);
    }

    private IEnumerable<IEnumerable<Action>> MoveCamera(RoomExit.RoomExitEventArgs obj)
    {
        GameTimer.Multiplier = 0.1f;

        IEnumerable<Action> hori = null;
        IEnumerable<Action> vert = null;

        if (obj.Source.FromRoom.RoomDefinition.RoomX != obj.Source.ToRoom.RoomDefinition.RoomX)
        {
            if (obj.Source.Direction.x!=0 || obj.Source.ToRoom.RoomDefinition.RoomSize.x == 1)
            {
                hori = _camera.transform.GetAccessor()
                    .Position
                    .X
                    .SetTarget(obj.Source.ToRoom.RoomDefinition.RoomSize.x > 1 && obj.Source.Direction.x == 0 ? _player.transform.position.x : obj.Source.ToRoom.transform.position.x)
                    .Over(0.4f)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .UsingTimer(_uiTimer)
                    .Build();
            }
            else
            {
                _cameraFollow.IsXBlocked = false;
            }
        }
        

        if (obj.Source.FromRoom.RoomDefinition.RoomY != obj.Source.ToRoom.RoomDefinition.RoomY)
        {
            if (obj.Source.Direction.y != 0 || obj.Source.ToRoom.RoomDefinition.RoomSize.y == 1)
            {
                vert = _camera.transform.GetAccessor()
                    .Position
                    .Y
                    .SetTarget(obj.Source.ToRoom.transform.position.y)
                    .Over(0.4f)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .UsingTimer(_uiTimer)
                    .Build();
            }
            else
            {
                _cameraFollow.IsYBlocked = false;
            }
        }

        var combined = new[] { hori, vert }
            .Where(act => act != null)
            .Aggregate<IEnumerable<Action>, IEnumerable<Action>>(null, (current, action) =>
                current == null ? action : current.Combine(action));

        if (combined != null) yield return combined.Then(EnableFollow());

    }

    private IEnumerable<Action> EnableFollow()
    {
        yield return TimeYields.WaitOneFrame;
        _cameraFollow.IsXBlocked = _cameraFollow.IsYBlocked = false;
        GameTimer.Multiplier = 1;
    }
}
