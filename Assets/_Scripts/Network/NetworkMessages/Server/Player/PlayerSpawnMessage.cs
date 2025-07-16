using System;
using UnityEngine;

[Serializable]
public class PlayerSpawnMessage : ServerMessage
{
    public string NetworkId;
    public Vector3 Position;
    
    public PlayerSpawnMessage() : base(NetworkMessageTypes.Player.Spawn) { }
}