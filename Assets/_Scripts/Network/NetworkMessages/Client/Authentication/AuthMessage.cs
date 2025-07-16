using System;

[Serializable]
public class AuthMessage : ClientMessage
{
    public string DeviceId;
    [JsonProperty("token")]
    public string Token;
    public AuthMessage() : base(NetworkMessageTypes.Authentication.PlayerTryAuth) { }
}