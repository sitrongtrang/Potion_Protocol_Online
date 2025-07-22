using System;
using Newtonsoft.Json;

[Serializable]
public class PongMessage : ServerMessage
{
    public long ClientSendTime;
    public long ServerReceiveTime;

    public PongMessage() : base(NetworkMessageTypes.Server.System.Pong) { }
}