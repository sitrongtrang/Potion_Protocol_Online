using System;

[Serializable]
public class LoginData
{
    [JsonProperty("username")]
    public string Username;
    [JsonProperty("password")]
    public string Password;
}