using System;
using UnityEngine;

[Serializable]
public class PlayerSpawnMessage : ServerMessage
{
    public Vector3 Position;
    
    public PlayerSpawnMessage() : base(NetworkMessageTypes.Server.Player.Spawn) { }
}