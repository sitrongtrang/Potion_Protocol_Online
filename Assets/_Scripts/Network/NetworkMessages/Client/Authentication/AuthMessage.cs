using System;
using Newtonsoft.Json;

[Serializable]
public class AuthMessage : ClientMessage
{
    public string Token;
    public AuthMessage() : base(NetworkMessageTypes.Client.Authentication.TryAuth) { }
}