using System;
using UnityEngine;

[Serializable]
public class PlayerConnectedMessage : ServerMessage
{
    public string PlayerName;
    public Vector3 SpawnPosition;

    public PlayerConnectedMessage() : base(NetworkMessageTypes.Player.Connected) { }
}