using System;
using UnityEngine;

[Serializable]
public class ItemDropMessage : ServerMessage
{
    public string ItemId;
    public Vector2 Position;

    public ItemDropMessage() : base(NetworkMessageTypes.Item.Drop) { }
}