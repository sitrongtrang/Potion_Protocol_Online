using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _remotePlayerPrefab;
    
    private void OnEnable()
    {
        NetworkEvents.OnPlayerSpawnRequested += TrySpawnPlayer;
    }
    
    private void OnDisable()
    {
        NetworkEvents.OnPlayerSpawnRequested -= TrySpawnPlayer;
    }
    
    private void TrySpawnPlayer(string networkId, Vector3 position, bool isLocal)
    {
        GameObject prefab = isLocal ? _localPlayerPrefab : _remotePlayerPrefab;
        
        if (prefab == null)
        {
            Debug.LogError($"Missing {(isLocal ? "local" : "remote")} player prefab");
            return;
        }
        
        GameObject playerObj = Instantiate(prefab, position, Quaternion.identity);
        
        if (!playerObj.TryGetComponent<NetworkIdentity>(out var identity))
        {
            Debug.LogError("Spawned player missing NetworkIdentity component");
            Destroy(playerObj);
            return;
        }
        
        identity.Initialize(networkId, isLocal);
        
        // Additional setup
        if (isLocal)
        {
            // Setup camera follow, input controls, etc.
            Debug.Log($"Spawned local player: {networkId}");
        }
        else
        {
            Debug.Log($"Spawned remote player: {networkId}");
        }
    }
}