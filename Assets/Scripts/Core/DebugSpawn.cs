using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpawn : MonoBehaviour
{
    public Room CurrentRoom;

    private void Awake()
    {
        Player.Instance().transform.position = transform.position;
        var roomManager = RoomManager.Instance();
        roomManager.CurrentRoom.Value = CurrentRoom.RoomDefinition;
        roomManager.KnownRooms.KnownRooms = roomManager.CurrentRoom.Value.RoomPositions;
    }
}
