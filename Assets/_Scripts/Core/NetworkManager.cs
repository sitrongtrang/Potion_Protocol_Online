using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    // Singleton instance
    public static NetworkManager Instance { get; private set; }

    [Header("Connection Settings")]
    [SerializeField] private string _ip = "127.0.0.1";
    [SerializeField] private int _port = 9000;
    [SerializeField] private float _reconnectDelay = 5f;

    [Header("Network Components")]
    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _receiveThread;
    private bool _isConnected = false;

    [Header("Client Identification")]
    private string _clientId; // Server-assigned ID
    public string ClientId => _clientId;
    private string _sessionToken; // For reconnection
    private string _authToken;
    private bool _isAuthenticated;

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }

        UnityMainThreadDispatcher.Initialize();
    }

    private void Initialize()
    {
        // PlayerPrefs.DeleteAll();
        // Load saved session token if exists
        // _sessionToken = PlayerPrefs.GetString("SessionToken");

    }

    private void Start() => ConnectToServer();

    private void OnDestroy()
    {
        Disconnect();
    }
    #endregion

    #region Connection Management

    public void ConnectToServer()
    {
        if (_isConnected) return;

        try
        {
            _client = new TcpClient();
            _client.Connect(_ip, _port);
            _client.NoDelay = true;
            _stream = _client.GetStream();
            _isConnected = true;

            _receiveThread = new Thread(ReceiveData);
            _receiveThread.IsBackground = true;
            _receiveThread.Start();

            Debug.Log($"Connected to {_ip}:{_port}");
            NetworkEvents.InvokeConnectionStatusChanged(true);

            // Start authentication process
            // Authenticate();
        }
        catch (Exception e)
        {
            Debug.LogError($"Connection failed: {e.Message}");
            ScheduleReconnect();
        }
    }

    public void Authenticate()
    {
        if (!string.IsNullOrEmpty(_sessionToken))
        {
            // Try to reconnect with existing session
            SendMessage(new ReconnectMessage
            {
                Token = PlayerPrefs.GetString("Token"),
                SessionToken = _sessionToken
            });
        }
        else
        {
            // New authentication
            // PlayerPrefs.SetString("Token", _authToken);
            SendMessage(new AuthMessage
            {
                Token = _authToken,
                DeviceId = SystemInfo.deviceUniqueIdentifier
            });
        }
    }

    public void Disconnect()
    {
        _isConnected = false;
        _isAuthenticated = false;
        _receiveThread?.Abort();

        try
        {
            lock (_stream)
            {
                _stream?.Close();
            }
            _client?.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Disconnect error: {e.Message}");
        }

        NetworkEvents.InvokeConnectionStatusChanged(false);
    }

    private void ScheduleReconnect()
    {
        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            Invoke(nameof(ConnectToServer), _reconnectDelay);
        });
    }
    #endregion

    #region Data Handling
    private void ReceiveData()
    {
        byte[] buffer = new byte[4096];
        while (_isConnected)
        {
            try
            {
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    try
                    {
                        ServerMessage message = Serialization.DeserializeMessage(buffer);
                        if (message != null)
                        {
                            UnityMainThreadDispatcher.Instance.Enqueue(() =>
                            {
                                ProcessIncomingMessage(message);
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Deserialization error: {ex}");
                    }
                }
            }
            catch (Exception e)
            {
                Thread.Sleep(100);
                if (_isConnected)
                {
                    Debug.LogError($"Receive error: {e.Message}");
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        Disconnect();
                        ScheduleReconnect();
                    });
                }
                break;
            }
        }
    }

    private void ProcessIncomingMessage(ServerMessage message)
    {
        // First handle system messages
        if (HandleSystemMessage(message))
            return;

        // Then dispatch to game systems
        NetworkEvents.InvokeMessageReceived(message);
    }

    public void SendMessage(ClientMessage message)
    {
        if (!_isConnected || _stream == null || !_stream.CanWrite)
            return;

        // Add client ID to all outgoing messages if authenticated
        if (_isAuthenticated && string.IsNullOrEmpty(message.SenderId))
        {
            message.SenderId = _clientId;
        }

        try
        {
            byte[] data = Serialization.SerializeMessage(message);
            lock (_stream)
            {
                _stream.Write(data, 0, data.Length);
                _stream.Flush();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Send error: {e.Message}");
            Disconnect();
            ScheduleReconnect();
        }
    }
    #endregion

    #region System Message Handlers
    private bool HandleSystemMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.System.AuthSuccess:
                HandleAuthSuccess((AuthSuccessMessage)message);
                return true;
            case NetworkMessageTypes.Server.System.Pong:
                NetworkTime.Instance?.HandlePong((PongMessage)message);
                return true;

            default:
                return false;
        }
    }

    private void HandleAuthSuccess(AuthSuccessMessage message)
    {
        _isAuthenticated = true;
        _sessionToken = message.ReconnectToken;
        _clientId = message.ReceiverId;
        // PlayerPrefs.SetString("SessionToken", _sessionToken);

        Debug.Log("Authentication successful");
    }

    #endregion

    #region Public
    public void SetAuthenToken(string authToken)
    {
        _authToken = authToken;
    }
    public void SetClientId(string clientId)
    {
        _clientId = clientId;
    }
    #endregion
}