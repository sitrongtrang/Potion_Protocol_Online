using System;

[Serializable]
public class PlayerDisconnectedMessage : ServerMessage
{
    public PlayerDisconnectedMessage() : base(NetworkMessageTypes.Player.Disconnected) { }
}