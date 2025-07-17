using System;
using UnityEngine;

[Serializable]
public class PlayerCollideMessage : ServerMessage
{
    public string Tag;
    public bool IsEntering; // true for OnTriggerEnter2D, false for OnTriggerExit2D
    public float CollidePositionX;
    public float CollidePositionY;
    
    public PlayerCollideMessage() : base(NetworkMessageTypes.Server.Player.Collide) { }
}