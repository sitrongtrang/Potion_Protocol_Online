using System;

[Serializable]
public class PongMessage : ServerMessage
{
    [JsonProperty("clientSendTime")]
    public double ClientSendTime;
    [JsonProperty("serverReceiveTime")]
    public double ServerReceiveTime;
    [JsonProperty("serverSendTime")]
    public double ServerSendTime;

    public PongMessage() : base(NetworkMessageTypes.Server.System.Pong) { }
}