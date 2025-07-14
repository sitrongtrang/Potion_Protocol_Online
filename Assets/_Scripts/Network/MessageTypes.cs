// Assets/Scripts/Network/NetworkMessages.cs
using System;
using UnityEngine;

[Serializable]
public abstract class NetworkMessage
{
    public string SenderId;
    public string MessageType { get; protected set; }
    public double Timestamp;

    public NetworkMessage()
    {
        Timestamp = Time.time;
    }
}

#region Client -> Server
#region Player Input Messages
[Serializable]
public class PlayerInputMessage : NetworkMessage
{
    public PlayerInputMessage() { MessageType = "PlayerInput"; }

    // Movement
    public Vector2 MoveDirection;    // Normalized (-1 to 1)
    public bool IsDashing;           // True when dash key pressed

    // Inventory
    public int SelectedSlot;         // 0-4 for inventory slots
    public bool PickupKeyDown;       // E pressed
    public bool DropKeyDown;         // Q pressed
    public bool PutToStationKeyDown;

    // Actions
    public bool AttackKeyDown;       // Left mouse down
    public bool CraftKeyDown;        // Space pressed
    public bool SubmitKeyDown;       // F pressed

    // Context (client's current state)
    public Vector2 CurrentPosition;  // For server reconciliation
    public Quaternion CurrentRotation;
}

// [Serializable]
// public class PlayerInventoryTransferMessage : NetworkMessage
// {
//     public PlayerInventoryTransferMessage() { MessageType = "InventoryTransfer"; }

//     public int FromSlot;
//     public int StationId;            // Only for station transfers
// }
#endregion
#endregion


#region Server -> All Client
[Serializable]
public class PlayerConnectedMessage : NetworkMessage
{
    public int playerId;
    public string playerName;
    public Vector3 spawnPosition;
}

[Serializable]
public class PlayerDisconnectedMessage : NetworkMessage
{
    public int playerId;
}

#region Player Action Messages (Server -> Clients)
[Serializable]
public class PlayerMoveMessage : NetworkMessage
{
    public PlayerMoveMessage() { MessageType = "PlayerMove"; }
    
    public string PlayerId;
    public Vector2 NewPosition;
    public Vector2 NewMovementDirection;
    public bool IsDashing;
}

[Serializable]
public class PlayerInventoryMessage : NetworkMessage
{
    public PlayerInventoryMessage() { MessageType = "PlayerInventoryAction"; }
    
    public string PlayerId;
    public string[] InventoryItems;
    public int SlotIndex;
    public string ActionType; // "Pickup", "Drop", "TransferToStation"
    public string ItemId; // Only for pickup/drop
    public Vector2 DropPosition; // Only for drop actions
}

[Serializable]
public class PlayerAttackMessage : NetworkMessage
{
    public PlayerAttackMessage() { MessageType = "PlayerAttack"; }
    
    public string PlayerId;
    public string[] TargetTypes; // "Enemy" or "Resource"
    public int[] TargetIds;
    public Vector2 AttackDirection;
}

[Serializable]
public class PlayerCraftMessage : NetworkMessage
{
    public PlayerCraftMessage() { MessageType = "PlayerCraft"; }
    
    public string PlayerId;
    public int StationId;
    public string[] InputItems; // Items currently in station
}

[Serializable]
public class PlayerSubmitMessage : NetworkMessage
{
    public PlayerSubmitMessage() { MessageType = "PlayerSubmit"; }
    
    public string PlayerId;
    public string ItemId;
    public int SubmissionPointId;
}
#endregion

#region Station Messages
[Serializable]
public class StationUpdateMessage : NetworkMessage
{
    public StationUpdateMessage() { MessageType = "StationUpdate"; }
    
    public int StationId;
    public string[] ItemIds;
    public bool CraftSuccess; // Only for craft results
    public string CraftedItemId; // Only if CraftSuccess=true
}
#endregion

#region Item Messages
[Serializable]
public class ItemDropMessage : NetworkMessage
{
    public string ItemId;
    public Vector2 Position;
}
#endregion

#region Enemy Messages
[Serializable]
public class EnemySpawnMessage : NetworkMessage
{
    public EnemySpawnMessage() { MessageType = "EnemySpawn"; }
    
    public int EnemyId;
    public string EnemyType;
    public int SpawnerId;
    public Vector2 Position;
    public Vector2 PatrolPoint;
}

[Serializable]
public class EnemyMoveMessage : NetworkMessage
{
    public EnemyMoveMessage() { MessageType = "EnemyMove"; }
    
    public int EnemyId;
    public Vector2 Position;
    public Vector2 MovementDirection;
}

[Serializable]
public class EnemyDeathMessage : NetworkMessage
{
    public EnemyDeathMessage() { MessageType = "EnemyDeath"; }
    
    public int EnemyId;
    public string KillerId;
    public Vector2 DeathPosition;
}
#endregion

#region Resource Messages
[Serializable]
public class ResourceSpawnMessage : NetworkMessage
{
    public ResourceSpawnMessage() { MessageType = "ResourceSpawn"; }
    
    public int ResourceId;
    public string ResourceType;
    public Vector2 Position;
}

[Serializable]
public class ResourceHarvestedMessage : NetworkMessage
{
    public ResourceHarvestedMessage() { MessageType = "ResourceHarvested"; }
    
    public int ResourceId;
    public string HarvesterId;
}
#endregion

#region Game State Messages
[Serializable]
public class GameScoreUpdateMessage : NetworkMessage
{
    public GameScoreUpdateMessage() { MessageType = "GameScoreUpdate"; }
    
    public string PlayerId;
    public int ScoreChange;
    public int NewTotalScore;
}

[Serializable]
public class GameTimeUpdateMessage : NetworkMessage
{
    public GameTimeUpdateMessage() { MessageType = "GameTimeUpdate"; }
    
    public float RemainingGameTime;
}
#endregion

#endregion



