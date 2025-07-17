using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _remotePlayerPrefab;
    private void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    private void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
    }

    private void TrySpawnPlayer(string networkId, Vector2 position, bool isLocal)
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

    [ContextMenu("Spawn")]
    private void RequestSpawnMessage()
    {
        NetworkManager.Instance.SendMessage(new PlayerSpawnRequest
        {
            
        });
    }

    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.Player.Spawn:
                HandlePlayerSpawn((PlayerSpawnMessage)message);
                break;
        }
    }

    private void HandlePlayerSpawn(PlayerSpawnMessage message)
    {
        TrySpawnPlayer(
            message.ReceiverId,
            new Vector2(message.PositionX, message.PositionY),
            message.ReceiverId == NetworkManager.Instance.ClientId
        );
    }
}