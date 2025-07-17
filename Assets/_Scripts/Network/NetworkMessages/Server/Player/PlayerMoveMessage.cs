using System;
using UnityEngine;

[Serializable]
public class PlayerMoveMessage : ServerMessage
{
    public Vector2 NewPosition;
    public Vector2 NewMovementDirection;
    public bool IsDashing;

    public PlayerMoveMessage() : base(NetworkMessageTypes.Server.Player.Movement) { }
}