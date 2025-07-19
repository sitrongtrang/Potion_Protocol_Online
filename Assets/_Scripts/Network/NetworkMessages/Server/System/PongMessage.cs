using System;
using Newtonsoft.Json;

[Serializable]
public class PongMessage : ServerMessage
{
    [JsonProperty("clientSendTime")]
    public long ClientSendTime;
    [JsonProperty("serverReceiveTime")]
    public long ServerReceiveTime;

    public PongMessage() : base(NetworkMessageTypes.Server.System.Pong) { }
}