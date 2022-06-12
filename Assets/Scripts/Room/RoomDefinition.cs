using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct RoomDefinition
{
    public bool Equals(RoomDefinition other)
    {
        return RoomX == other.RoomX && RoomY == other.RoomY;
    }

    public override bool Equals(object obj)
    {
        return obj is RoomDefinition other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (RoomX * 397) ^ RoomY;
        }
    }

    public int RoomX;
    public int RoomY;
    public Vector2Int RoomSize;
    public string RoomId => $"{RoomX}-{RoomY}";

    public Vector3 SpawnPosition;

    public Vector2Int[] RoomPositions
    {
        get
        {
            var room = this;
            return
                (from v in Enumerable.Range(room.RoomX, Math.Max(room.RoomSize.x, 1))
                 from v2 in Enumerable.Range(room.RoomY, Math.Max(room.RoomSize.y, 1))
                 select new Vector2Int(v, v2))
                    .ToArray();
        }
    }

    public static bool operator ==(RoomDefinition r1, RoomDefinition r2)
    {
        return r1.Equals(r2);
    }

    public static bool operator !=(RoomDefinition r1, RoomDefinition r2)
    {
        return !r1.Equals(r2);
    }
}

