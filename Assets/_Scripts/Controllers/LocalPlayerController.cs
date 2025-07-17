using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class LocalPlayerController : MonoBehaviour
{
    public string PlayerId { get; private set; }
    private InputManager _inputManager;
    public PlayerInventory Inventory { get; private set; }
    public PlayerInteraction Interaction { get; private set; }

    private Vector2 _newPosition;
    private NetworkIdentity _netIdentity;
    public NetworkIdentity NetIdentity => _netIdentity;
    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }
    void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
    }
    void Start()
    {
        _netIdentity = GetComponent<NetworkIdentity>();
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

    public void Initialize(InputManager inputManager)
    {
        Inventory = new PlayerInventory();
        Interaction = new PlayerInteraction();

        Inventory.Initialize(this, inputManager);
        Interaction.Initialize(this, inputManager);
    }

    private void HandleNetworkMessage(ServerMessage message)
    {
        var result = message.MessageType switch
        {
            NetworkMessageTypes.Server.Player.Movement => HandlePlayerMove(message),
            _ => null
        };
    }

    private object HandlePlayerMove(ServerMessage message)
    {
        _newPosition = ((PlayerMoveMessage)message).NewPosition;
        return null;
    }
}