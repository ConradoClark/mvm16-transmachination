using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomDefinition RoomDefinition;
    private List<RoomExit> _exits;

    private void OnEnable()
    {
        RoomManager.Instance().AddToManager(this);
    }

    private void OnDisable()
    {
        if (RoomManager.Instance() == null) return;
        RoomManager.Instance().RemoveFromManager(this);
    }

    public void AddExit(RoomExit exit)
    {
        _exits ??= new List<RoomExit>();
        _exits.Add(exit);
    }

    public IReadOnlyCollection<RoomExit> Exits => _exits.AsReadOnly();
}
