using System;

[Serializable]
public class LoginSuccess
{
    [JsonProperty("success")]
    public bool Success;
    [JsonProperty("message")]
    public string Message;
    [JsonProperty("data")]
    public LoginSuccessData LoginSuccessDat;

}

public class LoginSuccessData
{
    [JsonProperty("token")]
    public string Token;
    [JsonProperty("userId")]
    public string UserId;
}