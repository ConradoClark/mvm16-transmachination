using System.Linq;
using Licht.Impl.Events;
using UnityEngine;

public class Minimap : BaseObject
{
    public MinimapObjectPrefabPool MiniMapObjPool;
    public Vector2 ObjectDistance;
    public Vector2 Offset;
    private MinimapObject[] _mapObjects;

    private RoomManager _roomManager;
    private bool _enabled;

    protected void OnDestroy()
    {
        this.StopObservingEvent<RoomExit.RoomExitEvents, RoomExit.RoomExitEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnExit);
        this.StopObservingEvent<RoomExit.RoomExitEvents, RoomExit.RespawnEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnSpawn);
    }

    protected override void UnityAwake()
    {
        _roomManager = RoomManager.Instance();
        this.ObserveEvent<RoomExit.RoomExitEvents, RoomExit.RoomExitEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnExit);
        this.ObserveEvent<RoomExit.RoomExitEvents, RoomExit.RespawnEventArgs>(RoomExit.RoomExitEvents.OnRoomExit, OnSpawn);
    }

    private void OnSpawn(RoomExit.RespawnEventArgs obj)
    {
        var roomPos = new Vector2Int(obj.ToRoom.RoomX, obj.ToRoom.RoomY);
        if (!_roomManager.KnownRooms.KnownRoomsSet.Contains(roomPos))
        {
            _roomManager.KnownRooms.KnownRooms = _roomManager.KnownRooms.KnownRooms.Union(obj.ToRoom.RoomPositions).ToArray();
        }
        DrawMap();
    }

    private void OnExit(RoomExit.RoomExitEventArgs obj)
    {
        var roomPos = new Vector2Int(obj.Source.ToRoom.RoomDefinition.RoomX, obj.Source.ToRoom.RoomDefinition.RoomY);
        if (!_roomManager.KnownRooms.KnownRoomsSet.Contains(roomPos))
        {
            _roomManager.KnownRooms.KnownRooms = _roomManager.KnownRooms.KnownRooms.Union(obj.Source.ToRoom.RoomDefinition.RoomPositions).ToArray();
        }
        DrawMap();
    }

    private void OnEnable()
    {
        _enabled = true;
        if (MiniMapObjPool.TryGetManyFromPool(27, out var objects))
        {
            _mapObjects = objects;
            for (var i = 0; i < 3; i++)
                for (var j = 0; j < 9; j++)
                {
                    var obj = objects[j + i * 9];
                    obj.Component.transform.position = new Vector2(transform.position.x + Offset.x + ObjectDistance.x * j,
                        transform.position.y + Offset.y - ObjectDistance.y * i);
                }

            DrawMap();
        }
    }


    private void DrawMap()
    {
        if (!_enabled) return;

        var currentRoom = _roomManager.CurrentRoom;
        var current = _roomManager.GetRoom(currentRoom.Value.RoomX, currentRoom.Value.RoomY);

        var index = 0;
        for (var row = currentRoom.Value.RoomY + 1; row >= currentRoom.Value.RoomY -1; row--)
        {
            for (var i = currentRoom.Value.RoomX - 4; i < currentRoom.Value.RoomX + 5; i++)
            {
                var room = _roomManager.GetRoom(i, row);
                var minimapObject = _mapObjects[index];
                ProcessMapObject(room, minimapObject, current);

                if (!_roomManager.KnownRooms.KnownRoomsSet.Contains(new Vector2Int(i, row)))
                {
                    minimapObject.SetActive(false);
                }
                index++;
            }
        }
    }

    private bool ProcessMapObject(Room room, MinimapObject obj, Room playerPosition)
    {
        obj.SetActive(room != null);
        obj.SetRelevantPosition(room != null && room.RoomDefinition == playerPosition.RoomDefinition);

        return room != null && room.RoomDefinition == playerPosition.RoomDefinition;
    }
}
