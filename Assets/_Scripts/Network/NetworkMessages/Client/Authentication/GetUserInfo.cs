public class GetUserInfoClient : ClientMessage
{
    public GetUserInfoClient() : base(NetworkMessageTypes.Client.System.GetUserInfo) { }
}