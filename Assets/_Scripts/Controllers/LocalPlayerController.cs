using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class LocalPlayerController : MonoBehaviour
{
    [Header("Constants")]
    private const float CLIENT_SEND_INTERVAL = 1 / 30f;
    private const float SIM_TICK_INTERVAL = 1f / 60f;

    [Header("Components")]
    private float _simTimer = 0f;
    private float _sendTimer = 0f;
    public NetworkIdentity Identity { get; private set; }

    [Header("Syncing")]
    private bool _isReconciling = false;
    private PlayerInputSnapshot _inputListener = new();
    private NetworkPredictionBuffer<InputSnapshot, PlayerSnapshot> _networkPredictionBuffer = new(12);

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
        if (_inputManager != null && Identity.IsLocalPlayer)
        {
            _inputListener.MoveDir = _inputManager.controls.Player.Move.ReadValue<Vector2>().normalized;

            _inputListener.PickupPressed = _inputManager.controls.Player.Pickup.WasPressedThisFrame();
            _inputListener.DropPressed = _inputManager.controls.Player.Drop.WasPressedThisFrame();
            _inputListener.CombinePressed = _inputManager.controls.Player.Combine.WasPressedThisFrame();
            _inputListener.TransferPressed = _inputManager.controls.Player.Transfer.WasPressedThisFrame();
            _inputListener.SubmitPressed = _inputManager.controls.Player.Submit.WasPressedThisFrame();
        }

        _sendTimer += Time.deltaTime;
        while (_sendTimer >= CLIENT_SEND_INTERVAL)
        {
            _sendTimer -= CLIENT_SEND_INTERVAL;

            if (Identity.IsLocalPlayer)
            {

            }
            else
            {

            }
        }
    }

    void FixedUpdate()
    {
        _simTimer += Time.fixedDeltaTime;

        while (_simTimer >= SIM_TICK_INTERVAL)
        {
            _simTimer -= SIM_TICK_INTERVAL;

            if (Identity.IsLocalPlayer)
            {
                Simulate(_inputListener);
            }
            else
            {

            }
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

        // if (playerInputSnapshot.PickupPressed && TryPickup())
        //     return;

        // if (playerInputSnapshot.DropPressed && TryDrop())
        //     return;

        // if (playerInputSnapshot.CombinePressed && TryCraft())
        //     return;

        // if (playerInputSnapshot.TransferPressed && TryTransfer())
        //     return;

        // if (playerInputSnapshot.SubmitPressed && TrySubmit())
        //     return;

        if (cpy.MoveDir != Vector2.zero && TryMove(cpy))
            return;
    }

    private bool TryMove(PlayerInputSnapshot inputSnapshot)
    {
        inputSnapshot.InputSequence = _networkPredictionBuffer.GetCurrentInputSequence();

        transform.position = transform.position + (Vector3)(5 * SIM_TICK_INTERVAL * inputSnapshot.MoveDir);

        PlayerSnapshot playerSnapshot = new()
        {
            ProcessedInputSequence = inputSnapshot.InputSequence,
            Position = transform.position,
        };

        _networkPredictionBuffer.EnqueueInput(inputSnapshot);
        _networkPredictionBuffer.EnqueueState(playerSnapshot);

        return true;
    }
    #endregion

    #region Server Message
    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.Player.Movement:
                HandlePlayerMove((PlayerMoveMessage)message);
                break;
        }
    }

    private void HandlePlayerMove(PlayerMoveMessage message)
    {
        if (Identity.IsLocalPlayer)
        {
            TryReconcileServer(message);
        }
        else
        {

        }

    }
    private void TryReconcileServer(PlayerMoveMessage message)
    {
        int processedInputSequence = message.ProcessedInputSequence;

        StateSnapshot[] stateSnapshots = (StateSnapshot[])_networkPredictionBuffer.StateBufferAsArray.Clone();
        InputSnapshot[] inputSnapshots = (InputSnapshot[])_networkPredictionBuffer.InputBufferAsArray.Clone();

        bool needReconcile = false;
        int index;
        for (index = 0; index < stateSnapshots.Length; index++)
        {
            StateSnapshot stateSnapshot = stateSnapshots[index];
            if (stateSnapshot.ProcessedInputSequence == processedInputSequence)
            {
                Vector2 serverPlayerPosition = new Vector2(message.PositionX, message.PositionY);
                Vector2 historyPosition = ((PlayerSnapshot)stateSnapshot).Position;
                if (Vector2.Distance(serverPlayerPosition, historyPosition) >= 0.1f)
                    needReconcile = true;
                break;
            }
        }

        if (!needReconcile) return;

        index = FindFromIndex(inputSnapshots, index, processedInputSequence);
        if (index == -1) return;

        ReconcileServer(message, inputSnapshots, index + 1);
    }
    private void ReconcileServer(PlayerMoveMessage message, InputSnapshot[] inputSnapshots, int fromIndex)
    {
        _isReconciling = true;
        _networkPredictionBuffer.ClearStateBuffer();

        int processedInputSequence = message.ProcessedInputSequence;
        _networkPredictionBuffer.EnqueueState(new PlayerSnapshot
        {
            ProcessedInputSequence = processedInputSequence,
            Position = new(message.PositionX, message.PositionY)
        });

        transform.position = new Vector2(message.PositionX, message.PositionY);
        for (int i = fromIndex; i < inputSnapshots.Length; i++)
        {
            Vector2 moveDir = ((PlayerInputSnapshot)inputSnapshots[i]).MoveDir;
            transform.position = transform.position + (Vector3)(5f * SIM_TICK_INTERVAL * moveDir);

            _networkPredictionBuffer.EnqueueState(new PlayerSnapshot
            {
                ProcessedInputSequence = inputSnapshots[i].InputSequence,
                Position = transform.position
            });
        }

        _isReconciling = false;
    }
    #endregion

    #region Utilities
    int FindFromIndex(InputSnapshot[] inputSnapshots, int index, int targetSequence)
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
    #endregion
}