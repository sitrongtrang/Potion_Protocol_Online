using System;
using UnityEngine;

[Serializable]
public abstract class NetworkMessage
{
    public string SenderId;
    public short MessageType { get; protected set; }
    public double Timestamp;

    protected NetworkMessage(short messageType)
    {
        MessageType = messageType;
        Timestamp = Time.time;
    }
}

#region Client -> Server Messages (Input)
[Serializable]
public class PlayerAuthInputMessage : NetworkMessage
{
    public string token;
    public PlayerAuthInputMessage() : base(NetworkMessageTypes.Input.PlayerAuthTest) { }
}
[Serializable]
public class PlayerMoveInputMessage : NetworkMessage
{
    public Vector2 CurrentPosition;
    public Quaternion CurrentRotation;
    public Vector2 MoveDirection;
    public bool DashKeyDown;

    public PlayerMoveInputMessage() : base(NetworkMessageTypes.Input.PlayerMove) { }
}

[Serializable]
public class PlayerPickupInputMessage : NetworkMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool PickupKeyDown;

    public PlayerPickupInputMessage() : base(NetworkMessageTypes.Input.PlayerPickup) { }
}

[Serializable]
public class PlayerDropInputMessage : NetworkMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool DropKeyDown;

    public PlayerDropInputMessage() : base(NetworkMessageTypes.Input.PlayerDrop) { }
}

[Serializable]
public class PlayerTransferToStationInputMessage : NetworkMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool PutToStationKeyDown;

    public PlayerTransferToStationInputMessage() : base(NetworkMessageTypes.Input.PlayerTransferToStation) { }
}

[Serializable]
public class PlayerAttackInputMessage : NetworkMessage
{
    public Vector2 CurrentPosition;
    public Quaternion CurrentRotation;
    public bool AttackKeyDown;

    public PlayerAttackInputMessage() : base(NetworkMessageTypes.Input.PlayerAttack) { }
}

[Serializable]
public class PlayerCraftInputMessage : NetworkMessage
{
    public Vector2 CurrentPosition;
    public bool CraftKeyDown;

    public PlayerCraftInputMessage() : base(NetworkMessageTypes.Input.PlayerCraft) { }
}

[Serializable]
public class PlayerSubmitInputMessage : NetworkMessage
{
    public Vector2 CurrentPosition;
    public bool SubmitKeyDown;

    public PlayerSubmitInputMessage() : base(NetworkMessageTypes.Input.PlayerSubmit) { }
}
#endregion

#region Server -> Client Messages
[Serializable]
public class PlayerConnectedMessage : NetworkMessage
{
    public int PlayerId;
    public string PlayerName;
    public Vector3 SpawnPosition;

    public PlayerConnectedMessage() : base(NetworkMessageTypes.Player.Connected) { }
}

[Serializable]
public class PlayerDisconnectedMessage : NetworkMessage
{
    public int PlayerId;

    public PlayerDisconnectedMessage() : base(NetworkMessageTypes.Player.Disconnected) { }
}

[Serializable]
public class PlayerMoveMessage : NetworkMessage
{
    public string PlayerId;
    public Vector2 NewPosition;
    public Vector2 NewMovementDirection;
    public bool IsDashing;

    public PlayerMoveMessage() : base(NetworkMessageTypes.Player.Movement) { }
}

[Serializable]
public class PlayerInventoryMessage : NetworkMessage
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
public class PlayerAttackMessage : NetworkMessage
{
    public string PlayerId;
    public string[] TargetTypes;
    public int[] TargetIds;
    public Vector2 AttackDirection;

    public PlayerAttackMessage() : base(NetworkMessageTypes.Player.Attack) { }
}

[Serializable]
public class PlayerCraftMessage : NetworkMessage
{
    public string PlayerId;
    public int StationId;
    public string[] InputItems;
    public bool Success;
    public string CraftedItemId;

    public PlayerCraftMessage() : base(NetworkMessageTypes.Player.Craft) { }
}

[Serializable]
public class PlayerSubmitMessage : NetworkMessage
{
    public string PlayerId;
    public string ItemId;
    public int SubmissionPointId;

    public PlayerSubmitMessage() : base(NetworkMessageTypes.Player.Submit) { }
}

[Serializable]
public class StationUpdateMessage : NetworkMessage
{
    public int StationId;
    public string[] ItemIds;
    public bool CraftSuccess;
    public string CraftedItemId;

    public StationUpdateMessage() : base(NetworkMessageTypes.Station.Update) { }
}

[Serializable]
public class StationCraftMessage : NetworkMessage
{
    public int StationId;
    public float CraftTime;

    public StationCraftMessage() : base(NetworkMessageTypes.Station.Update) { }
}

[Serializable]
public class ItemDropMessage : NetworkMessage
{
    public string ItemId;
    public Vector2 Position;

    public ItemDropMessage() : base(NetworkMessageTypes.Item.Drop) { }
}

[Serializable]
public class EnemySpawnMessage : NetworkMessage
{
    public int EnemyId;
    public string EnemyType;
    public int SpawnerId;
    public Vector2 Position;
    public Vector2[] PatrolPoints;

    public EnemySpawnMessage() : base(NetworkMessageTypes.Enemy.Spawn) { }
}

[Serializable]
public class EnemyMoveMessage : NetworkMessage
{
    public int EnemyId;
    public Vector2 Position;
    public Vector2 MovementDirection;

    public EnemyMoveMessage() : base(NetworkMessageTypes.Enemy.Move) { }
}

[Serializable]
public class EnemyDeathMessage : NetworkMessage
{
    public int EnemyId;
    public string KillerId;
    public Vector2 DeathPosition;
    public string[] DroppedItems;

    public EnemyDeathMessage() : base(NetworkMessageTypes.Enemy.Death) { }
}

[Serializable]
public class ResourceSpawnMessage : NetworkMessage
{
    public int ResourceId;
    public string ResourceType;
    public Vector2 Position;

    public ResourceSpawnMessage() : base(NetworkMessageTypes.Resource.Spawn) { }
}

[Serializable]
public class ResourceHarvestedMessage : NetworkMessage
{
    public int ResourceId;
    public string HarvesterId;
    public string[] DroppedItems;

    public ResourceHarvestedMessage() : base(NetworkMessageTypes.Resource.Harvested) { }
}

[Serializable]
public class GameScoreUpdateMessage : NetworkMessage
{
    public string PlayerId;
    public int ScoreChange;
    public int NewTotalScore;

    public GameScoreUpdateMessage() : base(NetworkMessageTypes.GameState.ScoreUpdate) { }
}

[Serializable]
public class GameTimeUpdateMessage : NetworkMessage
{
    public float RemainingGameTime;
    public int CurrentWave;

    public GameTimeUpdateMessage() : base(NetworkMessageTypes.GameState.TimeUpdate) { }
}

[Serializable]
public class PingMessage : NetworkMessage
{
    public double SendTime;

    public PingMessage() : base(NetworkMessageTypes.System.Ping) { }
}

[Serializable]
public class PongMessage : NetworkMessage
{
    public double OriginalPingTime;

    public PongMessage() : base(NetworkMessageTypes.System.Pong) { }
}

[Serializable]
public class KickMessage : NetworkMessage
{
    public string Reason;

    public KickMessage() : base(NetworkMessageTypes.System.Kick) { }
}
#endregion