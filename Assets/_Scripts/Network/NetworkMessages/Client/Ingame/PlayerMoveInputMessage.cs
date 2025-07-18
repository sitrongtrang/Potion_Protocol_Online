using System;
using UnityEngine;

[Serializable]
public class PlayerMoveInputMessage : ClientMessage
{
    [JsonProperty("moveDirectionX")]
    public float MoveDirectionX;
    [JsonProperty("moveDirectionY")]
    public float MoveDirectionY;
    [JsonProperty("dashKeyDown")]
    public bool DashKeyDown;

    public PlayerMoveInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryMove) { }
}