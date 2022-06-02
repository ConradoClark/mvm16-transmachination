using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

public class RoomManager : SceneObject<RoomManager>
{
    public RoomScriptable CurrentRoom;
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
