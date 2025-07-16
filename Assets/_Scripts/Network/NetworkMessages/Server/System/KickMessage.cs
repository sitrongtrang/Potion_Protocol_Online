using System;

[Serializable]
public class KickMessage : ServerMessage
{
    public string Reason;

    public KickMessage() : base(NetworkMessageTypes.System.Kick) { }
}