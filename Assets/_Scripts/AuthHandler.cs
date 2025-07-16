using UnityEngine;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] private string _authToken;
    void Start()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }
    [ContextMenu("Test Auth")]
    private void TestSendAuth()
    {
        NetworkManager.Instance.SendMessage(new PlayerAuthInputMessage
        {
            Token = _authToken
        });

    }

    private void HandleNetworkMessage(ServerMessage message)
    {
        var result = message.MessageType switch
        {
            NetworkMessageTypes.Authorization.AuthSuccess => HandleAuthSucess(message),
            _ => null
        };
    }

    private object HandleAuthSucess(ServerMessage message)
    {
        PlayerAuthSucessMessage message1 = (PlayerAuthSucessMessage)message;
        Debug.Log("response: " + message1.Response + ", reconnectToken: " + message1.ReconnectToken);
        return null;
    }
}