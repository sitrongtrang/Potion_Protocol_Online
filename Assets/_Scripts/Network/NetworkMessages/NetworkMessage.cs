using System;
using Unity.Android.Gradle.Manifest;
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
    public long ClientSendTime;
    public int InputSequence;
    public long ClientEstimatedServerTime;
    protected ClientMessage(short messageType) : base(messageType) { ClientSendTime = TimeSyncUtils.GetUnixTimeMilliseconds(); }
}

public abstract class ServerMessage : NetworkMessage
{
    [JsonProperty("clientId")]
    public string ReceiverId;
    [JsonProperty("serverSendTime")]
    public long ServerSendTime;
    public int ProcessedInputSequence;
    protected ServerMessage(short messageType) : base(messageType) { }
}
