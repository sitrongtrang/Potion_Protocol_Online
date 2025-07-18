using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class LocalPlayerController : MonoBehaviour
{
    [Header("Constants")]
    private const int CLIENT_SEND_RATE = 20;
    private const int FIXED_UPDATE_RATE = 60;
    private const int SEND_EVERY_N_FIXED_UPDATES = FIXED_UPDATE_RATE / CLIENT_SEND_RATE; // 3

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Syncing")]
    private int fixedUpdateCounter = 0;
    private Queue<InputSnapshot> inputBuffer = new Queue<InputSnapshot>();
    private Queue<PlayerMoveMessage> receivedStates = new Queue<PlayerMoveMessage>();

    private PlayerState predictedState;
    private PlayerState lastConfirmedState;
    private long lastConfirmedTick = 0;
    private int clientTick = 0;

    private PlayerInterpolationData thisPlayerInterpolation = new PlayerInterpolationData();

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
        Time.fixedDeltaTime = 1f / FIXED_UPDATE_RATE;

        Identity = GetComponent<NetworkIdentity>();

        predictedState = new PlayerState
        {
            Position = transform.position,
        };
    }
    void Update()
    {
        float deltaTime = Time.deltaTime;

        if (Identity.IsLocalPlayer)
        {
            _moveDir = _inputManager.controls.Player.Move.ReadValue<Vector2>().normalized;
            updateVisualRepresentation();
        }
        else
        {
            processAsRemotePlayer(deltaTime);
        }
        processServerStates();
        
    }

    void FixedUpdate()
    {
        if (!Identity.IsLocalPlayer) return;

        float fixedDeltaTime = Time.fixedDeltaTime;
        clientTick += 1;

        HandleInput(fixedDeltaTime);

        fixedUpdateCounter++;
        if (fixedUpdateCounter >= SEND_EVERY_N_FIXED_UPDATES)
        {
            sendInputToServer();
            fixedUpdateCounter = 0;
        }
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
    private void HandleInput(float fixedDeltaTime)
    {
        InputSnapshot input = new InputSnapshot
        {
            horizontal = _moveDir.x,
            vertical = _moveDir.y,
            clientSendTime = TimeSyncUtils.GetUnixTimeMilliseconds(),
            clientEstimatedServerTime = NetworkTime.Instance.GetServerTime(),
            clientTick = clientTick
        };

        inputBuffer.Enqueue(input);

        while (inputBuffer.Count > FIXED_UPDATE_RATE)
        {
            inputBuffer.Dequeue();
        }

        predictedState = simulateMovement(predictedState, input, fixedDeltaTime);
    }
    private PlayerState simulateMovement(PlayerState currentState, InputSnapshot input, float deltaTime)
    {
        PlayerState newState = new PlayerState
        {
            Position = currentState.Position,
        };

        Vector2 inputVector = new Vector2(input.horizontal, input.vertical);
        Vector2 movement = deltaTime * moveSpeed * inputVector;

        newState.Position += movement;

        return newState;
    }
    private void updateVisualRepresentation()
    {
        transform.position = predictedState.Position;
    }
    private void processServerStates()
    {
        while (receivedStates.Count > 0)
        {
            PlayerMoveMessage serverState = receivedStates.Dequeue();
            if (Identity.IsLocalPlayer)
                processAsLocalPlayer(serverState);
            else
                storeForInterpolation(serverState);
        }
    }
    private void processAsLocalPlayer(PlayerMoveMessage serverState)
    {
        if (serverState.ServerTick > lastConfirmedTick)
        {
            PlayerState serverPlayerState = new PlayerState
            {
                Position = new Vector2(serverState.PositionX, serverState.PositionY)
            };

            lastConfirmedState = serverPlayerState;
            lastConfirmedTick = serverState.ServerTick;

            float positionError = Vector2.Distance(predictedState.Position, serverPlayerState.Position);
            if (positionError > 0.1f)
            {
                Debug.Log($"[Reconciliation] Position error: {positionError:F3}, correcting...");
                reconcileWithServer(serverPlayerState, serverState.ServerSendTime);
            }

        }
    }
    private void reconcileWithServer(PlayerState serverState, long serverTime)
    {
        predictedState = serverState;
        
        InputSnapshot[] replayInputs = inputBuffer.ToArray();
        
        foreach (InputSnapshot input in replayInputs)
        {
            if (input.clientSendTime > serverTime)
            {
                float fixedDeltaTime = Time.fixedDeltaTime;
                predictedState = simulateMovement(predictedState, input, fixedDeltaTime);
            }
        }
    }
    private void storeForInterpolation(PlayerMoveMessage serverState)
    {
        PlayerState state = new PlayerState
        {
            Position = new Vector2(serverState.PositionX, serverState.PositionY)
        };
        thisPlayerInterpolation.AddState(state, serverState.ServerSendTime);
    }
    private void processAsRemotePlayer(float deltaTime)
    {
        long renderTime = NetworkTime.Instance.GetServerTime() - 100;

        PlayerStateSnapshot fromState = null;
        PlayerStateSnapshot toState = null;

        for (int i = 0; i < thisPlayerInterpolation.StateHistory.Count - 1; i++)
        {
            if (thisPlayerInterpolation.StateHistory[i].Timestamp <= renderTime &&
                thisPlayerInterpolation.StateHistory[i + 1].Timestamp >= renderTime)
            {
                fromState = thisPlayerInterpolation.StateHistory[i];
                toState = thisPlayerInterpolation.StateHistory[i + 1];
                break;
            }
        }

        if (fromState != null && toState != null)
        {
            float timeDiff = (float)(toState.Timestamp - fromState.Timestamp);
            float t = (float)(renderTime - fromState.Timestamp) / timeDiff;

            Vector2 interpolatedPos = Vector2.Lerp(fromState.State.Position, toState.State.Position, t);
            transform.position = interpolatedPos;
        }
    }

    private void sendInputToServer()
    {
        if (inputBuffer.Count > 0)
        {
            InputSnapshot latestInput = inputBuffer.Last();

            PlayerMoveInputMessage inputMsg = new PlayerMoveInputMessage
            {
                MoveDirectionX = latestInput.horizontal,
                MoveDirectionY = latestInput.vertical,
                ClientTick = clientTick,
                ClientEstimatedServerTime = latestInput.clientEstimatedServerTime
            };

            NetworkManager.Instance.SendMessage(inputMsg);
        }
    }
    #endregion


    #region Server Message
    private void HandleNetworkMessage(ServerMessage message)
    {
        var result = message.MessageType switch
        {
            NetworkMessageTypes.Server.Player.Movement => HandlePlayerMove((PlayerMoveMessage)message),
            _ => null
        };
    }

    private object HandlePlayerMove(PlayerMoveMessage message)
    {
        if (message.ReceiverId == Identity.ClientId) receivedStates.Enqueue(message);
        return null;
    }
    #endregion
}