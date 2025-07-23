using System;
using Newtonsoft.Json;

[Serializable]
public class RegisterData
{
    [JsonProperty("username")]
    public string Username;
    [JsonProperty("password")]
    public string Password;
    [JsonProperty("confirmPassword")]
    public string ConfirmPassword;
    [JsonProperty("displayName")]
    public string DisplayName;
}