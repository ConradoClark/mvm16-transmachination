using System;
using Licht.Impl.Events;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameSpawn : SceneObject<GameSpawn>
{
    public SavePoint SavePoint;
    public SavePoint CheckPoint;
    public RoomScriptable CurrentRoom;

    private IEventPublisher<RoomExit.RoomExitEvents, RoomExit.RespawnEventArgs> _eventPublisher;

    private void Awake()
    {
        _eventPublisher = this.RegisterAsEventPublisher<RoomExit.RoomExitEvents, RoomExit.RespawnEventArgs>();
        CheckPoint.LoadFromSavePoint(SavePoint);
        SavePoint.Spawn();
    }

    public void SpawnFromCheckpoint()
    {
        _eventPublisher.PublishEvent(RoomExit.RoomExitEvents.OnRoomExit, new RoomExit.RespawnEventArgs
        {
            FromRoom = CurrentRoom.Value,
            ToRoom = CheckPoint.Room,
        });
        CheckPoint.Spawn();
    }

    public void SpawnFromLastSave()
    {
        _eventPublisher.PublishEvent(RoomExit.RoomExitEvents.OnRoomExit, new RoomExit.RespawnEventArgs
        {
            FromRoom = CurrentRoom.Value,
            ToRoom = SavePoint.Room,
        });
        SavePoint.Spawn();
    }

}
