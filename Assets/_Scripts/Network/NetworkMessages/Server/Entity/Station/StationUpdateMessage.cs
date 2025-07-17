using System;
using UnityEngine;

[Serializable]
public class StationUpdateMessage : ServerMessage
{
    public int StationId;
    public string[] ItemIds;
    public bool CraftSuccess;
    public string CraftedItemId;
    public float DropPositionX;
    public float DropPositionY;

    public StationUpdateMessage() : base(NetworkMessageTypes.Server.Station.Update) { }
}