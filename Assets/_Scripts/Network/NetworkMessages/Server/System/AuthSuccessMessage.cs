using System;
using Newtonsoft.Json;

[Serializable]
public class AuthSuccessMessage : ServerMessage
{
    [JsonProperty("clientId")]
    public string ClientId;
    [JsonProperty("response")]
    public string Response;
    [JsonProperty("reconnectToken")]
    public string ReconnectToken;
    public AuthSuccessMessage() : base(NetworkMessageTypes.Server.System.AuthSuccess) { }
}