using System;
using UnityEngine;

[Serializable]
public class PlayerCraftMessage : ServerMessage
{
    public int StationId;
    public string[] InputItems;
    public bool Success;
    public string CraftedItemId;

    public PlayerCraftMessage() : base(NetworkMessageTypes.Server.Player.Craft) { }
}