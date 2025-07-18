using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class LocalPlayerController : MonoBehaviour
{
    [Header("Constants")]
    private const int CLIENT_SEND_RATE = 20;
    private const int FIXED_UPDATE_RATE = 60;
    private const int SEND_EVERY_N_FIXED_UPDATES = FIXED_UPDATE_RATE / CLIENT_SEND_RATE; // 3
    
    [Header("Components")]
    public NetworkIdentity Identity { get; private set; }
    
    [Header("Movement")]
    private Vector2 _moveDir;
    private InputManager _inputManager;

    [Header("Game Components")]
    public PlayerInventory Inventory { get; private set; }
    public PlayerInteraction Interaction { get; private set; }

    #region Unity Lifecycle
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
        Identity = GetComponent<NetworkIdentity>();
    }
    void Update()
    {
        if (!Identity.IsLocalPlayer) return;
        _moveDir = _inputManager.controls.Player.Move.ReadValue<Vector2>().normalized;
    }

    void FixedUpdate()
    {
        if (Identity.IsLocalPlayer)
            HandleLocalMovemnt();
        else
            HandleRemoteMovement();
    }
    #endregion

    #region Initialize
    public void Initialize(InputManager inputManager)
    {
        _inputManager = inputManager;

        Inventory = new PlayerInventory();
        Interaction = new PlayerInteraction();

        Inventory.Initialize(this, inputManager);
        Interaction.Initialize(this, inputManager);
    }
    #endregion

    #region Client State
    private void HandleLocalMovemnt()
    {

    }
    private void HandleRemoteMovement()
    {

    }
    #endregion


    #region Server Message
    private void HandleNetworkMessage(ServerMessage message)
    {
        
    }
    #endregion
}