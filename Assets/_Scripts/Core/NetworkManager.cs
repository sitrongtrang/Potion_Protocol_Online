using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _receiveThread;
    private bool _isConnected = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ConnectToServer(string ip, int port)
    {
        try
        {
            _client = new TcpClient();
            _client.Connect(ip, port);
            _stream = _client.GetStream();
            _isConnected = true;

            _receiveThread = new Thread(ReceiveData)
            {
                IsBackground = true
            };
            _receiveThread.Start();

            Debug.Log("Connected to server");
        }
        catch (Exception e)
        {
            Debug.LogError($"Connection error: {e.Message}");
        }
    }

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
                            NetworkEvents.InvokeMessageReceived(message));
                    }
                }
            }
            catch (IOException)
            {
                Debug.Log("Disconnected from server");
                _isConnected = false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Receive error: {e.Message}");
                _isConnected = false;
            }
        }
    }

    public void SendMessage(NetworkMessage message)
    {
        if (!_isConnected) return;

        try
        {
            byte[] data = Serialization.SerializeMessage(message);
            _stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"Send error: {e.Message}");
            _isConnected = false;
        }
    }

    private void OnDestroy()
    {
        _isConnected = false;
        _receiveThread?.Abort();
        _client?.Close();
    }
}