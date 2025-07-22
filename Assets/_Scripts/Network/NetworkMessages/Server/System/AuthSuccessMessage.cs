using System;
using Newtonsoft.Json;

[Serializable]
public class AuthSuccessMessage : ServerMessage
{
    public string ClientId;
    public string Response;
    public string ReconnectToken;
    public AuthSuccessMessage() : base(NetworkMessageTypes.Server.System.AuthSuccess) { }
}