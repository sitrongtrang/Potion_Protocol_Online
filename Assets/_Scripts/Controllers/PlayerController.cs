using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 _newPosition;
    void Start()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }
    void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        // NetworkManager.Instance.SendMessage(new PlayerMoveInputMessage
        // {
        //     CurrentPosition = transform.position,
        //     CurrentRotation = transform.rotation,
        //     DashKeyDown = false,
        //     MoveDirection = new Vector2(x, y).normalized,
        // });
    }

    void FixedUpdate()
    {
        transform.position = _newPosition;
    }

    private void HandleNetworkMessage(ServerMessage message)
    {
        var result = message.MessageType switch
        {
            NetworkMessageTypes.Player.Movement => HandlePlayerMove(message),
            _ => null
        };
    }

    private object HandlePlayerMove(NetworkMessage message)
    {
        _newPosition = ((PlayerMoveMessage)message).NewPosition;
        return null;
    }
}