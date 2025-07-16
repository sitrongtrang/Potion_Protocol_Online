using UnityEngine;
using UnityEngine.Events;

public static class NetworkEvents
{
    public static event UnityAction<ServerMessage> OnMessageReceived;
    public static event UnityAction<bool> OnConnectionStatusChanged;

    public delegate void PlayerSpawnHandler(string networkId, Vector3 position, bool isLocal);
    public static event PlayerSpawnHandler OnPlayerSpawnRequested;
    public static void InvokePlayerSpawnRequested(string networkId, Vector3 position, bool isLocal)
    {
        OnPlayerSpawnRequested?.Invoke(networkId, position, isLocal);
    }

    public static void InvokeMessageReceived(ServerMessage message)
    {
        OnMessageReceived?.Invoke(message);
    }

    public static void InvokeConnectionStatusChanged(bool connected)
    {
        OnConnectionStatusChanged?.Invoke(connected);
    }
}