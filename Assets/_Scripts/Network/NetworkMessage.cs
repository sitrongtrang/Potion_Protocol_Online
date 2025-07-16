using System;
using UnityEngine;

[Serializable]
public abstract class NetworkMessage
{
    public short MessageType { get; protected set; }
    
    protected NetworkMessage(short messageType)
    {
        MessageType = messageType;
    }
}

public abstract class ClientMessage : NetworkMessage
{
    public string SenderId;
    public double Timestamp;
    protected ClientMessage(short messageType) : base(messageType) { Timestamp = Time.time; }
}

public abstract class ServerMessage : NetworkMessage
{
    public string ReceiverId;
    public int StatusCode;
    protected ServerMessage(short messageType) : base(messageType) { }
}



#region Client -> Server Messages (Input)
[Serializable]
public class PlayerAuthInputMessage : ClientMessage
{
    [JsonProperty("token")]
    public string Token;
    public PlayerAuthInputMessage() : base(NetworkMessageTypes.Input.PlayerTryAuth) { }
}
[Serializable]
public class PlayerMoveInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public Quaternion CurrentRotation;
    public Vector2 MoveDirection;
    public bool DashKeyDown;

    public PlayerMoveInputMessage() : base(NetworkMessageTypes.Input.PlayerTryMove) { }
}

[Serializable]
public class PlayerPickupInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool PickupKeyDown;

    public PlayerPickupInputMessage() : base(NetworkMessageTypes.Input.PlayerTryPickup) { }
}

[Serializable]
public class PlayerDropInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool DropKeyDown;

    public PlayerDropInputMessage() : base(NetworkMessageTypes.Input.PlayerTryDrop) { }
}

[Serializable]
public class PlayerTransferToStationInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool PutToStationKeyDown;

    public PlayerTransferToStationInputMessage() : base(NetworkMessageTypes.Input.PlayerTryTransferToStation) { }
}

[Serializable]
public class PlayerAttackInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public Quaternion CurrentRotation;
    public bool AttackKeyDown;

    public PlayerAttackInputMessage() : base(NetworkMessageTypes.Input.PlayerTryAttack) { }
}

[Serializable]
public class PlayerCraftInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public bool CraftKeyDown;

    public PlayerCraftInputMessage() : base(NetworkMessageTypes.Input.PlayerTryCraft) { }
}

[Serializable]
public class PlayerSubmitInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public bool SubmitKeyDown;

    public PlayerSubmitInputMessage() : base(NetworkMessageTypes.Input.PlayerTrySubmit) { }
}
#endregion

#region Server -> Client Messages
[Serializable]
public class PlayerAuthSucessMessage : ServerMessage
{
    [JsonProperty("response")]
    public string Response;
    [JsonProperty("reconnectToken")]
    public string ReconnectToken;
    public PlayerAuthSucessMessage() : base(NetworkMessageTypes.Authorization.AuthSuccess) { }
}
public class PlayerConnectedMessage : ServerMessage
{
    public int PlayerId;
    public string PlayerName;
    public Vector3 SpawnPosition;

    public PlayerConnectedMessage() : base(NetworkMessageTypes.Player.Connected) { }
}

[Serializable]
public class PlayerDisconnectedMessage : ServerMessage
{
    public int PlayerId;

    public PlayerDisconnectedMessage() : base(NetworkMessageTypes.Player.Disconnected) { }
}

[Serializable]
public class PlayerMoveMessage : ServerMessage
{
    public string PlayerId;
    public Vector2 NewPosition;
    public Vector2 NewMovementDirection;
    public bool IsDashing;

    public PlayerMoveMessage() : base(NetworkMessageTypes.Player.Movement) { }
}

[Serializable]
public class PlayerInventoryMessage : ServerMessage
{
    public string PlayerId;
    public string[] InventoryItems;
    public int SlotIndex;
    public string ActionType; // "Pickup", "Drop", "TransferToStation"
    public string ItemId;
    public Vector2 DropPosition;

    public PlayerInventoryMessage() : base(NetworkMessageTypes.Player.Inventory) { }
}

[Serializable]
public class PlayerAttackMessage : ServerMessage
{
    public string PlayerId;
    public string[] TargetTypes;
    public int[] TargetIds;
    public Vector2 AttackDirection;

    public PlayerAttackMessage() : base(NetworkMessageTypes.Player.Attack) { }
}

[Serializable]
public class PlayerCraftMessage : ServerMessage
{
    public string PlayerId;
    public int StationId;
    public string[] InputItems;
    public bool Success;
    public string CraftedItemId;

    public PlayerCraftMessage() : base(NetworkMessageTypes.Player.Craft) { }
}

[Serializable]
public class PlayerSubmitMessage : ServerMessage
{
    public string PlayerId;
    public string ItemId;
    public int SubmissionPointId;

    public PlayerSubmitMessage() : base(NetworkMessageTypes.Player.Submit) { }
}

[Serializable]
public class StationUpdateMessage : ServerMessage
{
    public int StationId;
    public string[] ItemIds;
    public bool CraftSuccess;
    public string CraftedItemId;

    public StationUpdateMessage() : base(NetworkMessageTypes.Station.Update) { }
}

[Serializable]
public class StationCraftMessage : ServerMessage
{
    public int StationId;
    public float CraftTime;

    public StationCraftMessage() : base(NetworkMessageTypes.Station.Update) { }
}

[Serializable]
public class ItemDropMessage : ServerMessage
{
    public string ItemId;
    public Vector2 Position;

    public ItemDropMessage() : base(NetworkMessageTypes.Item.Drop) { }
}

[Serializable]
public class EnemySpawnMessage : ServerMessage
{
    public int EnemyId;
    public string EnemyType;
    public int SpawnerId;
    public Vector2 Position;
    public Vector2[] PatrolPoints;

    public EnemySpawnMessage() : base(NetworkMessageTypes.Enemy.Spawn) { }
}

[Serializable]
public class EnemyMoveMessage : ServerMessage
{
    public int EnemyId;
    public Vector2 Position;
    public Vector2 MovementDirection;

    public EnemyMoveMessage() : base(NetworkMessageTypes.Enemy.Move) { }
}

[Serializable]
public class EnemyDeathMessage : ServerMessage
{
    public int EnemyId;
    public string KillerId;
    public Vector2 DeathPosition;
    public string[] DroppedItems;

    public EnemyDeathMessage() : base(NetworkMessageTypes.Enemy.Death) { }
}

[Serializable]
public class ResourceSpawnMessage : ServerMessage
{
    public int ResourceId;
    public string ResourceType;
    public Vector2 Position;

    public ResourceSpawnMessage() : base(NetworkMessageTypes.Resource.Spawn) { }
}

[Serializable]
public class ResourceHarvestedMessage : ServerMessage
{
    public int ResourceId;
    public string HarvesterId;
    public string[] DroppedItems;

    public ResourceHarvestedMessage() : base(NetworkMessageTypes.Resource.Harvested) { }
}

[Serializable]
public class GameScoreUpdateMessage : ServerMessage
{
    public string PlayerId;
    public int ScoreChange;
    public int NewTotalScore;

    public GameScoreUpdateMessage() : base(NetworkMessageTypes.GameState.ScoreUpdate) { }
}

[Serializable]
public class GameTimeUpdateMessage : ServerMessage
{
    public float RemainingGameTime;
    public int CurrentWave;

    public GameTimeUpdateMessage() : base(NetworkMessageTypes.GameState.TimeUpdate) { }
}

[Serializable]
public class PingMessage : ServerMessage
{
    public double SendTime;

    public PingMessage() : base(NetworkMessageTypes.System.Ping) { }
}

[Serializable]
public class PongMessage : ServerMessage
{
    public double OriginalPingTime;

    public PongMessage() : base(NetworkMessageTypes.System.Pong) { }
}

[Serializable]
public class KickMessage : ServerMessage
{
    public string Reason;

    public KickMessage() : base(NetworkMessageTypes.System.Kick) { }
}
#endregion