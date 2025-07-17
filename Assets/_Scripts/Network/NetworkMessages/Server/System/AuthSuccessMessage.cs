using System;

[Serializable]
public class AuthSuccessMessage : ServerMessage
{
    [JsonProperty("response")]
    public string Response;
    [JsonProperty("reconnectToken")]
    public string ReconnectToken;
    public AuthSuccessMessage() : base(NetworkMessageTypes.Server.System.AuthSuccess) { }
}