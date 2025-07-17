using System;
using UnityEngine;

[Serializable]
public class PlayerCraftInputMessage : ClientMessage
{
    public float CurretPositionX;
    public float CurrentPositionY;
    public bool CraftKeyDown;

    public PlayerCraftInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryCraft) { }
}