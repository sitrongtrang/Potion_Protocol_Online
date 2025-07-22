using System;
using Newtonsoft.Json;

[Serializable]
public class LoginSuccess
{
    public bool Success;
    public string Message;
    public LoginSuccessData LoginSuccessDat;

}

public class LoginSuccessData
{
    public string Token;
    public string UserId;
}