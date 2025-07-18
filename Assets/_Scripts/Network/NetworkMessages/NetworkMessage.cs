using System;
using UnityEngine;

[Serializable]
public abstract class NetworkMessage
{
    public short MessageType { get; protected set; }
    
    protected NetworkMessage(short messageType)
    {
        MessageType = messageType;
    }
}

public abstract class ClientMessage : NetworkMessage
{
    [JsonProperty("clientId")]
    public string SenderId;
    [JsonProperty("clientSendTime")]
    public double ClientSendTime;
    protected ClientMessage(short messageType) : base(messageType) { ClientSendTime = TimeSyncUtils.GetUnixTimeMilliseconds(); }
}

public abstract class ServerMessage : NetworkMessage
{
    [JsonProperty("clientId")]
    public string ReceiverId;
    public int StatusCode;
    [JsonProperty("serverTime")]
    public double ServerSendTime;
    protected ServerMessage(short messageType) : base(messageType) { }
}
