using System;
using UnityEngine;

[Serializable]
public class PlayerMoveInputMessage : ClientMessage
{
    [JsonProperty("currentPosition")]
    public Vector2 CurrentPosition;
    public Quaternion CurrentRotation;
    [JsonProperty("moveDirection")]
    public Vector2 MoveDirection;
    [JsonProperty("dashKeyDown")]
    public bool DashKeyDown;

    public PlayerMoveInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryMove) { }
}