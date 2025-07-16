using System.Collections.Generic;
using UnityEngine;

public class NetworkIdentity : MonoBehaviour
{
    private static readonly Dictionary<string, NetworkIdentity> _allClients = new();
    
    public string ClientId { get; private set; }
    public bool IsLocalPlayer { get; private set; }

    public void Initialize(string clientId, bool isLocal)
    {
        ClientId = clientId;
        IsLocalPlayer = isLocal;
        
        if (_allClients.ContainsKey(clientId))
        {
            Debug.LogError($"Duplicate client ID detected: {clientId}");
            return;
        }
        
        _allClients[clientId] = this;
    }

    public static bool TryGetPlayer(string clientId, out NetworkIdentity identity)
    {
        return _allClients.TryGetValue(clientId, out identity);
    }

    void OnDestroy()
    {
        _allClients.Remove(ClientId);
    }
}