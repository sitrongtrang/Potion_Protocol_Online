using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerController : MonoBehaviour
{
    
    [Header("Components")]
    private float _sendTimer = 0f;
    public NetworkIdentity Identity { get; private set; }

    [Header("Syncing")]
    private PlayerNetworkSimulator _simulator = new(NetworkConstants.NET_PRED_BUFFER_SIZE);
    private PlayerNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);
    // private int _serverSequence = int.MaxValue;
    // private bool _isReconciling = false;
    private PlayerInputSnapshot _inputListener = new();
    // private NetworkPredictionBuffer<PlayerInputMessage, PlayerSnapshot> _networkPredictionBuffer = new(NetworkConstants.NET_PRED_BUFFER_SIZE);
    // private NetworkInterpolationBuffer<PlayerStateInterpolateData> _networkInterpolationBuffer = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);

    [Header("Input")]
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
    void Awake()
    {
        Application.runInBackground = true;
        Identity = GetComponent<NetworkIdentity>();
    }
    void Update()
    {
        if (!Identity.IsLocalPlayer) return;

        if (_inputManager != null)
        {
            _inputListener.MoveDir = _inputManager.controls.Player.Move.ReadValue<Vector2>().normalized;

            _inputListener.AttackPressed = _inputManager.controls.Player.Attack.WasPressedThisFrame();
            _inputListener.DashPressed = _inputManager.controls.Player.Dash.WasPressedThisFrame();
            _inputListener.PickupPressed = _inputManager.controls.Player.Pickup.WasPressedThisFrame();
            _inputListener.DropPressed = _inputManager.controls.Player.Drop.WasPressedThisFrame();
            _inputListener.CombinePressed = _inputManager.controls.Player.Combine.WasPressedThisFrame();
            _inputListener.TransferPressed = _inputManager.controls.Player.Transfer.WasPressedThisFrame();
            _inputListener.SubmitPressed = _inputManager.controls.Player.Submit.WasPressedThisFrame();
        }

        _sendTimer += Time.deltaTime;
        while (_sendTimer >= NetworkConstants.NET_TICK_INTERVAL)
        {
            _sendTimer -= NetworkConstants.NET_TICK_INTERVAL;

            NetworkManager.Instance.SendMessage(new BatchPlayerInputMessage
            {
                PlayerInputMessages = _simulator.InputBufferAsArray
            });
        }
    }

    void FixedUpdate()
    {
        if (Identity.IsLocalPlayer)
        {
            Simulate(_inputListener);
        }
        else
        {
            _interpolator.Interpolate((serverState) =>
            {
                transform.position = new(serverState.PositionX, serverState.PositionY);
            });
        }
    }
    #endregion

    #region Initialization
    public void Initialize(InputManager inputManager, string id, bool isLocal)
    {
        _inputManager = inputManager;
        Identity.Initialize(id, isLocal);

        Inventory = new PlayerInventory();
        Interaction = new PlayerInteraction();

        Inventory.Initialize(this, inputManager);
        Interaction.Initialize(this, inputManager);
    }
    #endregion

    #region Simulation
    private void Simulate(PlayerInputSnapshot inputSnapshot)
    {
        PlayerInputSnapshot cpy = new(inputSnapshot);

        // if (cpy.PickupPressed && TryPickup())
        //     return;

        // if (cpy.DropPressed && TryDrop())
        //     return;

        // if (cpy.CombinePressed && TryCraft())
        //     return;

        // if (cpy.TransferPressed && TryTransfer())
        //     return;

        // if (cpy.SubmitPressed && TrySubmit())
        //     return;

        // if (cpy.MoveDir != Vector2.zero && TryDash(cpy))
        //     return;

        if (cpy.MoveDir != Vector2.zero && TryMove(cpy))
            return;
    }

    private bool TryMove(PlayerInputSnapshot inputSnapshot)
    {
        _simulator.Simulate(inputSnapshot,
            (inputSnapshot) =>
            {
                transform.position = transform.position + (Vector3)(5 * Time.fixedDeltaTime * inputSnapshot.MoveDir);
                return new()
                {
                    Position = transform.position,
                };
            }
        );
        return true;
    }
    #endregion

    #region Server Message
    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.GameState.StateUpdate:
                GameStatesUpdate gameStatesUpdate = (GameStatesUpdate)message;
                if (Identity.IsLocalPlayer)
                {
                    GameStateUpdate gameStateUpdate = gameStatesUpdate.GameStates[FindServerLastProcessedInputIndex(gameStatesUpdate)];
                    foreach (PlayerState playerState in gameStateUpdate.PlayerStates)
                        if (playerState.PlayerId == Identity.ClientId)
                        {
                            TryReconcileServer(new PlayerSnapshot()
                            {
                                ProcessedInputSequence = gameStateUpdate.ProcessedInputSequence,
                                Position = new(playerState.PositionX, playerState.PositionY),
                            });
                            break;
                        }

                }
                else
                {
                    _interpolator.Store(gameStatesUpdate.GameStates, (gameStates) =>
                    {
                        for (int i = 0; i < gameStates.PlayerStates.Length; i++)
                        {
                            if (gameStates.PlayerStates[i].PlayerId == Identity.ClientId)
                            {
                                return i;
                            }
                        }
                        return -1;
                    });
                }
                break;
                    
        }
    }

    private void TryReconcileServer(PlayerSnapshot state)
    {
        _simulator.Reconcile(state,
            (serverSnapshot, historySnapshot) =>
            {
                return Vector2.Distance(serverSnapshot.Position, historySnapshot.Position) > 0.1f;
            },
            (snapshot) =>
            {
                transform.position = snapshot.Position;
            },
            (inputMessage) =>
            {
                Vector2 moveDir = new(inputMessage.MoveDirX, inputMessage.MoveDirY);
                return new PlayerSnapshot()
                {
                    Position = transform.position + (Vector3)(5f * Time.fixedDeltaTime * moveDir)
                };
            }
        );
    }
    #endregion

    #region Utilities
    private int FindServerLastProcessedInputIndex(GameStatesUpdate gameStatesUpdate)
    {
        int index = 0;
        int lastProcessedInputIndex = int.MinValue;
        for (int i = 0; i < gameStatesUpdate.GameStates.Length; i++)
        {
            if (gameStatesUpdate.GameStates[i].ProcessedInputSequence > lastProcessedInputIndex)
            {
                lastProcessedInputIndex = gameStatesUpdate.GameStates[i].ProcessedInputSequence;
                index = i;
            }
        }
        return index;
    }
    #endregion
}