using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Objects;
using UnityEngine;

public class RoomManager : SceneObject<RoomManager>
{
    public RoomScriptable CurrentRoom;
    public KnownRoomsScriptable KnownRooms;
    private readonly Lazy<Dictionary<RoomDefinition, Room>> _rooms = new Lazy<Dictionary<RoomDefinition, Room>>(() => new Dictionary<RoomDefinition, Room>());

    public void AddToManager(Room room)
    {
        _rooms.Value[room.RoomDefinition] = room;
    }

    public void RemoveFromManager(Room room)
    {
        _rooms.Value.Remove(room.RoomDefinition);
    }

    public Room GetRoom(int x, int y)
    {
        return _rooms.Value.FirstOrDefault(room => room.Key.RoomPositions.Contains(new Vector2Int(x, y)))
            .Value;
    }
}
