using UnityEngine;
using UnityEngine.Events;

public static class NetworkEvents
{
    public static event UnityAction<ServerMessage> OnMessageReceived;
    public static event UnityAction<bool> OnConnectionStatusChanged;

    public static void InvokeMessageReceived(ServerMessage message)
    {
        OnMessageReceived?.Invoke(message);
    }

    public static void InvokeConnectionStatusChanged(bool connected)
    {
        OnConnectionStatusChanged?.Invoke(connected);
    }
}