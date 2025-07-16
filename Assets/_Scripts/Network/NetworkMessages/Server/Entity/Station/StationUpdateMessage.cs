using System;

[Serializable]
public class StationUpdateMessage : ServerMessage
{
    public int StationId;
    public string[] ItemIds;
    public bool CraftSuccess;
    public string CraftedItemId;

    public StationUpdateMessage() : base(NetworkMessageTypes.Station.Update) { }
}