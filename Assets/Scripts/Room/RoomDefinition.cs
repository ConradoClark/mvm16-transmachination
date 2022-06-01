using System;

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
    public string RoomId => $"{RoomX}-{RoomY}";

    public static bool operator ==(RoomDefinition r1, RoomDefinition r2)
    {
        return r1.Equals(r2);
    }

    public static bool operator !=(RoomDefinition r1, RoomDefinition r2)
    {
        return !r1.Equals(r2);
    }
}

