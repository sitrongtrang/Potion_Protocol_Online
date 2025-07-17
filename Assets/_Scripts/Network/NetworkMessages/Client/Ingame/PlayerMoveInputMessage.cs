using System;
using UnityEngine;

[Serializable]
public class PlayerMoveInputMessage : ClientMessage
{
    [JsonProperty("currentPositionX")]
    public float CurrentPositionX;
    [JsonProperty("currentPositionY")]
    public float CurrentPositionY;
    // public Quaternion CurrentRotation;
    [JsonProperty("moveDirectionX")]
    public float MoveDirectionX;
    [JsonProperty("moveDirectionY")]
    public float MoveDirectionY;
    [JsonProperty("dashKeyDown")]
    public bool DashKeyDown;

    public PlayerMoveInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryMove) { }
}