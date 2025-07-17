using System;

[Serializable]
public class PingMessage : ClientMessage
{
    public PingMessage() : base(NetworkMessageTypes.Client.System.Ping) { }
}