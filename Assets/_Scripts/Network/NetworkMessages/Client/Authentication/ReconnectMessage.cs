using System;
using Newtonsoft.Json;

[Serializable]
public class ReconnectMessage : ClientMessage
{
    [JsonProperty("token")]
    public string Token;
    [JsonProperty("reconnectToken")]
    public string SessionToken;
    public ReconnectMessage() : base(NetworkMessageTypes.Client.Authentication.TryReconnect) { }
}