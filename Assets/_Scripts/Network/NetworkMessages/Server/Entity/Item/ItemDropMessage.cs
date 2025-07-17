using System;
using UnityEngine;

[Serializable]
public class ItemDropMessage : ServerMessage
{
    public string ItemId;
    public float PositionX;
    public float PositionY;

    public ItemDropMessage() : base(NetworkMessageTypes.Server.Item.Drop) { }
}