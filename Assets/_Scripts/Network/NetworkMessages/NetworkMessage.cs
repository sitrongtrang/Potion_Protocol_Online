using System;
using Newtonsoft.Json;

[Serializable]
public abstract class NetworkMessage
{
    public short MessageType { get; protected set; }
    
    protected NetworkMessage(short messageType)
    {
        MessageType = messageType;
    }
}

[Serializable]
public abstract class ClientMessage : NetworkMessage
{
    [JsonProperty("clientId")]
    public string SenderId;
    [JsonProperty("clientSendTime")]
    public long ClientSendTime;
    [JsonProperty("clientEstimatedServerTime")]
    public long ClientEstimatedServerTime;
    protected ClientMessage(short messageType) : base(messageType)
    {
        ClientSendTime = TimeSyncUtils.GetUnixTimeMilliseconds();
        if (NetworkTime.Instance != null)
            ClientEstimatedServerTime = NetworkTime.Instance.EstimatedServerTime;
    }
}

[Serializable]
public abstract class ServerMessage : NetworkMessage
{
    [JsonProperty("serverSendTime")]
    public long ServerSendTime;
    protected ServerMessage(short messageType) : base(messageType) { }
}

[Serializable]
public class BatchPlayerInputMessage : ClientMessage
{
    [JsonProperty("playerInputs")]
    public PlayerInputMessage[] PlayerInputMessages;
    public BatchPlayerInputMessage() : base(NetworkMessageTypes.Client.Ingame.Input) { }
}
