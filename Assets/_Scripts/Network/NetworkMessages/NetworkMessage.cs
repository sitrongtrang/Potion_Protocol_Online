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
    public string SenderId;
    public double Timestamp;
    protected ClientMessage(short messageType) : base(messageType) { Timestamp = Time.time; }
}

public abstract class ServerMessage : NetworkMessage
{
    public string ReceiverId;
    public int StatusCode;
    public double ServerTimeStamp;
    protected ServerMessage(short messageType) : base(messageType) { }
}
