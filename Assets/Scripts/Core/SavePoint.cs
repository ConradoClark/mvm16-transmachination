using System;
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
    public ScriptableForm.CharacterForm HeadForm;
    public bool Created;
    public int Slot;

    [Serializable]
    public struct SerializableTrigger
    {
        public bool Triggered{ get; set; }
        public string Name { get; set; }
    }

    public struct SavePointStruct
    {
        public SerializableTrigger[] Triggers;
        public RoomDefinition Room;
        public Vector2Int[] KnownRooms;
        public ScriptableForm.CharacterForm HeadForm;
        public bool Created;
        public int Slot;
    }

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
        HeadForm = savePoint.HeadForm;
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
        player.Form.Eyes.Form = HeadForm;
        player.UpdateForm();

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
            SavePointStruct
        {
            Triggers= Triggers.Select(t=> new SerializableTrigger
            {
                Triggered = t.Enabled,
                Name = t.Trigger.name,
            }).ToArray(),
            Room = Room,
            KnownRooms = KnownRooms,
            HeadForm = HeadForm
        }, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
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

    public static void SaveFile(int slot, SavePoint savePoint)
    {
        var path = $"{Application.persistentDataPath}/saveFile{slot}.dat";
        System.IO.File.WriteAllText(path, savePoint.Serialize());
    }

    public void Deserialize(string saveFile)
    {
        var savePoint = JsonConvert.DeserializeObject<SavePointStruct>(saveFile);
        HeadForm = savePoint.HeadForm;
        KnownRooms = savePoint.KnownRooms;
        Room = savePoint.Room;
        Triggers = savePoint.Triggers.SelectMany(t =>
        {
            var trigger = AllTriggers.Triggers.FirstOrDefault(trigger => t.Name == trigger.name);

            if (trigger == null) return Enumerable.Empty<TriggerSettings>();

            return new[]
            {
                new TriggerSettings
                {
                    Enabled = t.Triggered,
                    Trigger = trigger,
                }
            };
        }).ToArray();
    }

}
