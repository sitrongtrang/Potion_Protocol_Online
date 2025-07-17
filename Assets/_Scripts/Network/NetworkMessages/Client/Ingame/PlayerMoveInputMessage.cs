using System;
using UnityEngine;

[Serializable]
public class PlayerMoveInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public Quaternion CurrentRotation;
    public Vector2 MoveDirection;
    public bool DashKeyDown;

    public PlayerMoveInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryMove) { }
}