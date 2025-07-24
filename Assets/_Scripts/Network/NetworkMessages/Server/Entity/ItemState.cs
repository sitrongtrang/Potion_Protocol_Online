using System;

[Flags]
public enum PositionFlag
{
    None = 0,
    OnGround = 1 << 0,
    Inventory = 1 << 1,
    Station = 1 << 2,
    InObject = 1 << 3,
}

public class ItemState
{
    public string ItemId;
    public int PositionFlag;
    public float PositionX;
    public float PositionY;
    public int Slot;
    public string StationId;
}