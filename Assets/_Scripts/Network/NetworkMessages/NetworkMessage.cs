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
    public double ClientSendTime;
    [JsonProperty("clientTick")]
    public int ClientTick;
    protected ClientMessage(short messageType) : base(messageType) { ClientSendTime = TimeSyncUtils.GetUnixTimeMilliseconds(); }
}

public abstract class ServerMessage : NetworkMessage
{
    [JsonProperty("clientId")]
    public string ReceiverId;
    [JsonProperty("serverSendTime")]
    public double ServerSendTime;
    [JsonProperty("serverTick")]
    public int ServerTick;
    protected ServerMessage(short messageType) : base(messageType) { }
}
