using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KnownRooms", menuName = "TM/Room/KnownRooms", order = 1)]
public class KnownRoomsScriptable : ScriptableObject
{
    public Vector2Int[] KnownRooms;

    private HashSet<Vector2Int> _knownRoomsSet;
    public HashSet<Vector2Int> KnownRoomsSet
    {
        get
        {
            if (_knownRoomsSet?.Count == KnownRooms.Length) return _knownRoomsSet ??= new HashSet<Vector2Int>(KnownRooms ?? Array.Empty<Vector2Int>());
            
            _knownRoomsSet = new HashSet<Vector2Int>(KnownRooms);

            return _knownRoomsSet;
        }
    }
}

