using System;
using UnityEngine;

[Serializable]
public class PlayerConnectedMessage : ServerMessage
{
    public PlayerConnectedMessage() : base(NetworkMessageTypes.Server.Player.Connected) { }
}