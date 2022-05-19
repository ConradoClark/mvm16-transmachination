using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : ManagerObject<RoomManager>
{
    private readonly Lazy<Dictionary<RoomDefinition, Room>> _rooms = new Lazy<Dictionary<RoomDefinition, Room>>(() => new Dictionary<RoomDefinition, Room>());

    public void AddToManager(Room room)
    {
        _rooms.Value[room.RoomDefinition] = room;
    }

    public void RemoveFromManager(Room room)
    {
        _rooms.Value.Remove(room.RoomDefinition);
    }
}
