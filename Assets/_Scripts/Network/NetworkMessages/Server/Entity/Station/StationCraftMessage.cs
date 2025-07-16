using System;

[Serializable]
public class StationCraftMessage : ServerMessage
{
    public int StationId;
    public float CraftTime;

    public StationCraftMessage() : base(NetworkMessageTypes.Station.Update) { }
}