using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerController : MonoBehaviour
{
    
    [Header("Components")]
    private float _sendTimer = 0f;
    public NetworkIdentity Identity { get; private set; }

    [Header("Syncing")]
    private int _serverSequence = int.MaxValue;
    private bool _isReconciling = false;
    private PlayerInputSnapshot _inputListener = new();
    private NetworkPredictionBuffer<PlayerInputMessage, PlayerSnapshot> _networkPredictionBuffer = new(NetworkConstants.NET_PRED_BUFFER_SIZE);
    private NetworkInterpolationBuffer<PlayerStateInterpolateData> _networkInterpolationBuffer = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);

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
        // Application.runInBackground = true;
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
                PlayerInputMessages = _networkPredictionBuffer.InputBufferAsArray
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
            TryInterpolate();
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
        if (_isReconciling) return;

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

        if (cpy.MoveDir != Vector2.zero && TryMove(cpy))
            return;
    }

    private bool TryMove(PlayerInputSnapshot inputSnapshot)
    {
        int inputSequence = _networkPredictionBuffer.GetCurrentInputSequence();

        transform.position = transform.position + (Vector3)(5 * Time.fixedDeltaTime * inputSnapshot.MoveDir);

        PlayerInputMessage inputMesasage = new(inputSnapshot)
        {
            InputSequence = inputSequence
        };

        PlayerSnapshot playerSnapshot = new()
        {
            ProcessedInputSequence = inputSequence,
            Position = transform.position,
        };

        _networkPredictionBuffer.EnqueueInput(inputMesasage);
        _networkPredictionBuffer.EnqueueState(playerSnapshot);

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
                        TryReconcileServer(playerState, gameStateUpdate.ProcessedInputSequence);
                        break;
                    }

                }
                else
                {
                    StoreForInterpolate(gameStatesUpdate);
                }
                break;
                    
        }
    }

    private void TryReconcileServer(PlayerState state, int processedInputSequence)
    {
        PlayerSnapshot[] stateSnapshots = (PlayerSnapshot[])_networkPredictionBuffer.StateBufferAsArray.Clone();
        PlayerInputMessage[] inputSnapshots = (PlayerInputMessage[])_networkPredictionBuffer.InputBufferAsArray.Clone();

        bool needReconcile = false;
        int index;
        for (index = 0; index < stateSnapshots.Length; index++)
        {
            PlayerSnapshot stateSnapshot = stateSnapshots[index];
            if (stateSnapshot.ProcessedInputSequence == processedInputSequence)
            {
                Vector2 serverPlayerPosition = new Vector2(state.PositionX, state.PositionY);
                Vector2 historyPosition = stateSnapshot.Position;
                if (Vector2.Distance(serverPlayerPosition, historyPosition) >= 0.1f)
                    needReconcile = true;
                break;
            }
        }

        if (!needReconcile) return;

        index = FindFromIndex(inputSnapshots, index, processedInputSequence);
        if (index == -1) return;

        ReconcileServer(state, processedInputSequence, inputSnapshots, index + 1);
    }
    private void ReconcileServer(PlayerState state, int processedInputSequence, PlayerInputMessage[] inputSnapshots, int fromIndex)
    {
        _isReconciling = true;
        _networkPredictionBuffer.ClearStateSnapshot();

        _networkPredictionBuffer.EnqueueState(new PlayerSnapshot
        {
            ProcessedInputSequence = processedInputSequence,
            Position = new(state.PositionX, state.PositionY)
        });

        transform.position = new Vector2(state.PositionX, state.PositionY);
        for (int i = fromIndex; i < inputSnapshots.Length; i++)
        {
            Vector2 moveDir = new(inputSnapshots[i].MoveDirX, inputSnapshots[i].MoveDirY);
            transform.position = transform.position + (Vector3)(5f * Time.fixedDeltaTime * moveDir);

            _networkPredictionBuffer.EnqueueState(new PlayerSnapshot
            {
                ProcessedInputSequence = inputSnapshots[i].InputSequence,
                Position = transform.position
            });
        }

        _isReconciling = false;
    }
    private void StoreForInterpolate(GameStatesUpdate gameStatesUpdate)
    {
        bool inInitializing = _serverSequence == int.MaxValue;

        for (int i = 0; i < gameStatesUpdate.GameStates.Count; i++)
        {
            GameStateUpdate currGameState = gameStatesUpdate.GameStates[i];
            for (int j = 0; j < currGameState.PlayerStates.Length; j++)
            {
                if (currGameState.PlayerStates[j].PlayerId == Identity.ClientId)
                {
                    if (inInitializing)
                    {
                        if (currGameState.ServerSequence < _serverSequence)
                        {
                            _serverSequence = currGameState.ServerSequence;
                        }
                    }
                    else
                    {
                        if (currGameState.ServerSequence < _serverSequence)
                        {
                            break;
                        }
                        else
                        {
                            if (currGameState.ServerSequence - _serverSequence > NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE)
                            {
                                _serverSequence = currGameState.ServerSequence;
                            }
                        }
                    }
                    _networkInterpolationBuffer.Add(new PlayerStateInterpolateData()
                    {
                        ServerSequence = currGameState.ServerSequence,
                        PositionX = currGameState.PlayerStates[j].PositionX,
                        PositionY = currGameState.PlayerStates[j].PositionY
                    });
                    break;
                }
            }
        }
    }
    private void TryInterpolate()
    {
        if (_networkInterpolationBuffer.Poll(_serverSequence, out PlayerStateInterpolateData result))
        {
            transform.position = new(result.PositionX, result.PositionY);
            _serverSequence += 1;
        }
    }
    #endregion

    #region Utilities
    int FindFromIndex(PlayerInputMessage[] inputSnapshots, int index, int targetSequence)
    {
        int length = inputSnapshots.Length;

        for (int i = index; i >= 0; i--)
        {
            if (inputSnapshots[i].InputSequence == targetSequence)
                return i;
            if (inputSnapshots[i].InputSequence < targetSequence)
                break;
        }

        for (int i = index + 1; i < length; i++)
        {
            if (inputSnapshots[i].InputSequence == targetSequence)
                return i;
            if (inputSnapshots[i].InputSequence > targetSequence)
                break;
        }

        return -1;

    }
    private int FindServerLastProcessedInputIndex(GameStatesUpdate gameStatesUpdate)
    {
        int index = 0;
        int lastProcessedInputIndex = int.MinValue;
        for (int i = 0; i < gameStatesUpdate.GameStates.Count; i++)
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