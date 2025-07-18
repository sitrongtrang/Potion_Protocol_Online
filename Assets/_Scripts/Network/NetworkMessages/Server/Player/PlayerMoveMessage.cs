using System;
using UnityEngine;

[Serializable]
public class PlayerMoveMessage : ServerMessage
{
    [JsonProperty("positionX")]
    public float PositionX;
    [JsonProperty("positionY")]
    public float PositionY;
    public bool IsDashing;

    public PlayerMoveMessage() : base(NetworkMessageTypes.Server.Player.Movement) { }
}