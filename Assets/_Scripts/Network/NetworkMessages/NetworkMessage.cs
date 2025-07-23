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
    protected ClientMessage(short messageType) : base(messageType) {  }
}

[Serializable]
public abstract class ServerMessage : NetworkMessage
{
    protected ServerMessage(short messageType) : base(messageType) { }
}

[Serializable]
public class BatchPlayerInputMessage : ClientMessage
{
    public PlayerInputMessage[] PlayerInputMessages;
    public BatchPlayerInputMessage() : base(NetworkMessageTypes.Client.Ingame.Input) { }
}
