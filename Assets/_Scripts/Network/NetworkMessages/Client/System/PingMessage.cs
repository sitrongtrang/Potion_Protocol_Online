using System;

[Serializable]
public class PingMessage : ClientMessage
{
    [FieldOrder(0)]
    public long ClientSendTime;
    [FieldOrder(1)]
    public long ClientEstimatedServerTime;
    public PingMessage() : base(NetworkMessageTypes.Client.System.Ping)
    { 
        ClientSendTime = TimeSyncUtils.GetUnixTimeMilliseconds();
        if (NetworkTime.Instance != null)
            ClientEstimatedServerTime = NetworkTime.Instance.EstimatedServerTime;
    }
}