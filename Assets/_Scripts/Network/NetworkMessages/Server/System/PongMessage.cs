using System;

[Serializable]
public class PongMessage : ServerMessage
{
    [JsonProperty("clientSendTime")]
    public double ClientSendTime;
    [JsonProperty("serverReceiveTime")]
    public double ServerReceiveTime;

    public PongMessage() : base(NetworkMessageTypes.Server.System.Pong) { }
}