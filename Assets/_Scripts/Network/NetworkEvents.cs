using System;
using UnityEngine.Events;

[Serializable]
public class NetworkMessageEvent : UnityEvent<NetworkMessage> {}

public static class NetworkEvents
{
    public static event Action<NetworkMessage> OnNetworkMessageReceived;

    public static void InvokeMessageReceived(NetworkMessage message)
    {
        OnNetworkMessageReceived?.Invoke(message);
    }
}