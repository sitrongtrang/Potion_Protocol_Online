using System;
using UnityEngine;

[Serializable]
public class PlayerCollideInputMessage : ClientMessage
{
    public string Tag;
    public bool IsEntering; // true for OnTriggerEnter2D, false for OnTriggerExit2D
    public float CurretPositionX;
    public float CurrentPositionY;
    public float CollidePositionX;
    public float CollidePositionY;

    public PlayerCollideInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryCollide) { }
}