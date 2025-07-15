using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [Header("Connection Settings")]
    [SerializeField] private string _ip = "127.0.0.1";
    [SerializeField] private int _port = 9000;
    [SerializeField] private float _reconnectDelay = 5f;
    [SerializeField] private string _authToken;
    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _receiveThread;
    private bool _isConnected = false;

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Initialize()
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ConnectToServer();
    }

    [ContextMenu("Test Auth")]
    private void TestSendAuth()
    {
        SendMessage(new PlayerAuthInputMessage
        {
            token = _authToken
        });

    }

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
        }
        catch (Exception e)
        {
            Debug.LogError($"Connection failed: {e.Message}");
            ScheduleReconnect();
        }
    }
    
    public void Disconnect()
    {
        _isConnected = false;
        _receiveThread?.Abort();
        
        try
        {
            _stream?.Close();
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
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Invoke(nameof(ConnectToServer), _reconnectDelay);
        });
    }
    #endregion

    #region Data Transmission
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
                    NetworkMessage message = Serialization.DeserializeMessage(buffer);
                    if (message != null)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            NetworkEvents.InvokeMessageReceived(message);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                if (_isConnected) // Only log if we expected to be connected
                {
                    Debug.LogError($"Receive error: {e.Message}");
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        Disconnect();
                        ScheduleReconnect();
                    });
                }
                break;
            }
        }
    }

    public void SendMessage(NetworkMessage message)
    {
        if (!_isConnected || _stream == null || !_stream.CanWrite) 
            return;

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

    // #region Initialization
    // private void Initialize()
    // {
    //     // Register core message handlers
    //     NetworkEvents.OnMessageReceived += HandleCoreMessages;
    //     NetworkEvents.OnConnectionStatusChanged += HandleConnectionChange;
    // }

    // private void HandleCoreMessages(NetworkMessage message)
    // {
    //     // Handle system-critical messages here
    //     switch (message.MessageType)
    //     {
    //         // case "Kick":
    //         //     HandleKickMessage((KickMessage)message);
    //         //     break;
    //         case "Ping":
    //             HandlePingMessage((PingMessage)message);
    //             break;
    //     }
    // }

    // private void HandleConnectionChange(bool connected)
    // {
    //     // Update UI or game state
    //     if (connected)
    //     {
    //         Debug.Log("Network connection established");
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Network connection lost");
    //     }
    // }
    // #endregion

    // #region System Message Handlers
    // private void HandleKickMessage(KickMessage message)
    // {
    //     Debug.Log($"Kicked from server: {message.Reason}");
    //     Disconnect();
    //     SceneLoader.LoadScene("MainMenu");
    // }

    // private void HandlePingMessage(PingMessage message)
    // {
    //     SendMessage(new PingMessage {
    //         Timestamp = message.Timestamp
    //     });
    // }
    // #endregion

    private void OnDestroy()
    {
        Disconnect();
        // NetworkEvents.OnMessageReceived -= HandleCoreMessages;
        // NetworkEvents.OnConnectionStatusChanged -= HandleConnectionChange;
    }
}