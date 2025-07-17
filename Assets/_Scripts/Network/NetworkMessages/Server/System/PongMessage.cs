using System;

[Serializable]
public class PongMessage : ServerMessage
{
    public double ClientSendTime;
    public double ServerReceiveTime;
    public double ServerSendTime;

    public PongMessage() : base(NetworkMessageTypes.Server.System.Pong) { }
}