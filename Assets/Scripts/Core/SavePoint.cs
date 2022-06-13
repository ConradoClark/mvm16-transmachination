using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "SavePoint", menuName = "TM/Saves/SavePoint", order = 1)]
public class SavePoint : ScriptableObject
{
    public AllTriggers AllTriggers;
    public TriggerSettings[] Triggers;
    public RoomDefinition Room;
    public Vector2Int[] KnownRooms;
    public bool Created;
    public int Slot;

    public void LoadFromSavePoint(SavePoint savePoint)
    {
        Triggers = savePoint.Triggers.Union(
            AllTriggers.Triggers.Where(t => Triggers.All(trigger => trigger.Trigger != t))
                .Select(t => new TriggerSettings
                {
                    Enabled = false,
                    Trigger = t
                })).ToArray();

        Room = savePoint.Room;
        KnownRooms = savePoint.KnownRooms;
        Created = savePoint.Created;
        Slot = savePoint.Slot;
    }

    public void Spawn()
    {
        Triggers = Triggers.Union(
            AllTriggers.Triggers.Where(t => Triggers.All(trigger => trigger.Trigger != t))
                .Select(t => new TriggerSettings
                {
                    Enabled = false,
                    Trigger = t
                })).ToArray();

        var player = Player.Instance();
        player.transform.position = Room.SpawnPosition;
        player.gameObject.SetActive(true);

        foreach (var trigger in Triggers)
        {
            trigger.Trigger.Triggered = trigger.Enabled;
        }

        var roomManager = RoomManager.Instance();
        roomManager.CurrentRoom.Value = Room;
        roomManager.KnownRooms.KnownRooms = KnownRooms;

        var roomPos = roomManager
            .GetRoom(Room.RoomX, Room.RoomY).transform.position;

        var gameCamera = GameCamera.Instance();
        gameCamera.transform.position = new Vector3(roomPos.x, roomPos.y, gameCamera.transform.position.z);
    }

    public string Serialize()
    {
        Triggers = Triggers.Union(
            AllTriggers.Triggers.Where(t => Triggers.All(trigger => trigger.Trigger != t))
                .Select(t => new TriggerSettings
                {
                    Enabled = false,
                    Trigger = t
                })).ToArray();

        var saveFile = JsonConvert.SerializeObject(new
        {
            Triggers,
            Room,
            KnownRooms
        });

        return saveFile;
    }

    public static string LoadFile(int slot)
    {
        var path = $"{Application.persistentDataPath}/saveFile{slot}.dat";
        if (!System.IO.File.Exists(path))
        {
            return null;
        }

        var file = System.IO.File.ReadAllText(path);
        return file;
    }

    public static void CreateSaveFile(int slot, SavePoint savePoint)
    {
        var path = $"{Application.persistentDataPath}/saveFile{slot}.dat";
        if (System.IO.File.Exists(path))
        {
            return;
        }

        System.IO.File.WriteAllText(path, savePoint.Serialize());
    }

    public void Deserialize(string saveFile)
    {
        var savePoint = JsonConvert.DeserializeObject<SavePoint>(saveFile);
        KnownRooms = savePoint.KnownRooms;
        Room = savePoint.Room;
        Triggers = savePoint.Triggers;
    }

}
