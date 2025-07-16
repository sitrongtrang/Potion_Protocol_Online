using System;

[Serializable]
public class ReconnectMessage : ClientMessage
{
    [JsonProperty("reconnectToken")]
    public string SessionToken;
    public ReconnectMessage() : base(NetworkMessageTypes.Authentication.PlayerTryReconnect) { }
}