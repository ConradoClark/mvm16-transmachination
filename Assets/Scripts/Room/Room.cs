using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomDefinition RoomDefinition;

    private void OnEnable()
    {
        RoomManager.Instance.AddToManager(this);
    }

    private void OnDisable()
    {
        if (RoomManager.Instance == null) return;
        RoomManager.Instance.RemoveFromManager(this);
    }
}
