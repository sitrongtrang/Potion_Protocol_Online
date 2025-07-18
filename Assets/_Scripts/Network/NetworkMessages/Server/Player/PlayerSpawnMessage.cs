using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class PlayerSpawnMessage : ServerMessage
{
    [JsonProperty("positionX")]
    public float PositionX;
    [JsonProperty("positionY")]
    public float PositionY;
    
    public PlayerSpawnMessage() : base(NetworkMessageTypes.Server.Player.Spawn) { }
}