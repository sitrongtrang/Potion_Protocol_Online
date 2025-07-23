using System;
using Newtonsoft.Json;

[Serializable]
public class RegisterSuccess
{
    [JsonProperty("success")]
    public bool Success;
    [JsonProperty("message")]
    public string Message;
}