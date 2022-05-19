using System;

[Serializable]
public struct RoomDefinition
{
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

