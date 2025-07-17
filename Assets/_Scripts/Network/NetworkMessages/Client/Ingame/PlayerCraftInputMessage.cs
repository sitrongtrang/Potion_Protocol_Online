using System;
using UnityEngine;

[Serializable]
public class PlayerCraftInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public bool CraftKeyDown;

    public PlayerCraftInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryCraft) { }
}