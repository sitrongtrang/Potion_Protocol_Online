using System;
using UnityEngine;

[Serializable]
public class PlayerSpawnMessage : ServerMessage
{
    public float PositionX;
    public float PositionY;
    
    public PlayerSpawnMessage() : base(NetworkMessageTypes.Server.Player.Spawn) { }
}