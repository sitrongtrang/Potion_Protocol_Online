using System;
using UnityEngine;

[Serializable]
public class PlayerCollideMessage : ClientMessage
{
    public string Tag;
    public bool IsEntering; // true for OnTriggerEnter2D, false for OnTriggerExit2D
    public Vector2 CollisionPosition;

    public PlayerCollideMessage() : base(NetworkMessageTypes.IngameInput.PlayerTryCollide) { }
}