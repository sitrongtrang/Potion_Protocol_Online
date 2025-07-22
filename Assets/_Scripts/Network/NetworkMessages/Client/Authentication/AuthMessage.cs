using System;
using Newtonsoft.Json;

[Serializable]
public class AuthMessage : ClientMessage
{
    [FieldOrder(0)]
    public string Token;
    public AuthMessage() : base(NetworkMessageTypes.Client.Authentication.TryAuth) { }
}