public class GetUserInfoServer : ServerMessage
{
    [FieldOrder(0)]
    public string ClientId;
    [FieldOrder(1)]
    public string Username;
    [FieldOrder(2)]
    public string DisplayName;
    [FieldOrder(3)]
    public int Level;

    public GetUserInfoServer() : base(NetworkMessageTypes.Server.System.GetUserInfo) { }
}