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

        var roomPos = roomManager
            .GetRoom(CurrentRoom.RoomDefinition.RoomX, CurrentRoom.RoomDefinition.RoomY).transform.position;

        var gameCamera = GameCamera.Instance();
        gameCamera.transform.position = new Vector3(roomPos.x, roomPos.y, gameCamera.transform.position.z);
    }
}
