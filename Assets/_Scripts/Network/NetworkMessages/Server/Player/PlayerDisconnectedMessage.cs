using System;

[Serializable]
public class PlayerDisconnectedMessage : ServerMessage
{
    public PlayerDisconnectedMessage() : base(NetworkMessageTypes.Server.Player.Disconnected) { }
}