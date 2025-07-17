using System;
using UnityEngine;

[Serializable]
public class PlayerMoveMessage : ServerMessage
{
    public float NewPositionX;
    public float NewPositionY;
    public float NewMovementDirectionX;
    public float NewMovementDirectionY;
    public bool IsDashing;

    public PlayerMoveMessage() : base(NetworkMessageTypes.Server.Player.Movement) { }
}