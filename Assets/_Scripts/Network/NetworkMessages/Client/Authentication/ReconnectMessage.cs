using System;

[Serializable]
public class ReconnectMessage : ClientMessage
{
    [JsonProperty("token")]
    public string Token;
    [JsonProperty("reconnectToken")]
    public string SessionToken;
    public ReconnectMessage() : base(NetworkMessageTypes.Authentication.PlayerTryReconnect) { }
}