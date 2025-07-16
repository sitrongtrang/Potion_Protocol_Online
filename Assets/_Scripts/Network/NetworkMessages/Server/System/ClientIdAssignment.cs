using System;

[Serializable]
public class ClientIdMessage : ServerMessage
{
    public string AssignedId;
    public ClientIdMessage() : base(NetworkMessageTypes.System.ClientIdAssignment) { }
}