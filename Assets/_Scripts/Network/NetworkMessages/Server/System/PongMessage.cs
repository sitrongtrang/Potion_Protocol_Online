using System;

[Serializable]
public class PongMessage : ServerMessage
{
    public double OriginalPingTime;

    public PongMessage() : base(NetworkMessageTypes.System.Pong) { }
}