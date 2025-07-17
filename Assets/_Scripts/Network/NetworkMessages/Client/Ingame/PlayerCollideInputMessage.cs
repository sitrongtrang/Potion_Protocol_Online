using System;
using UnityEngine;

[Serializable]
public class PlayerCollideInputMessage : ClientMessage
{
    public string Tag;
    public bool IsEntering; // true for OnTriggerEnter2D, false for OnTriggerExit2D
    public Vector2 CurrentPosition;
    public Vector2 CollisionPosition;

    public PlayerCollideInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryCollide) { }
}