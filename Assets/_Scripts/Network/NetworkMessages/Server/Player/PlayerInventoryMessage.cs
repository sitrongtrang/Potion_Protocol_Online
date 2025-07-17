using System;
using UnityEngine;

[Serializable]
public class PlayerInventoryMessage : ServerMessage
{
    public string[] InventoryItems;
    public int SlotIndex;
    public string AcTionType; // Pickup, Drop, Transfer, or Submit
    public string ItemId;
    public float DropPositionX;
    public float DropPositionY;

    public PlayerInventoryMessage() : base(NetworkMessageTypes.Server.Player.Inventory) { }
}