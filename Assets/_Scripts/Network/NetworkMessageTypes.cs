public static class NetworkMessageTypes
{
    // Client -> Server Messages
    public static class Input
    {
        public const string PlayerMove = "PlayerMoveInput";
        public const string PlayerPickup = "PlayerPickupInput";
        public const string PlayerDrop = "PlayerDropInput";
        public const string PlayerTransferToStation = "PlayerTransferToStationInput";
        public const string PlayerAttack = "PlayerAttackInput";
        public const string PlayerCraft = "PlayerCraftInput";
        public const string PlayerSubmit = "PlayerSubmitInput";
    }

    // Server -> Client Messages
    public static class Player
    {
        public const string Connected = "PlayerConnected";
        public const string Disconnected = "PlayerDisconnected";
        public const string Movement = "PlayerMovement";
        public const string Inventory = "PlayerInventoryAction";
        public const string Attack = "PlayerAttack";
        public const string Craft = "PlayerCraft";
        public const string Submit = "PlayerSubmit";
    }

    public static class Station
    {
        public const string Update = "StationUpdate";
        public const string Craft = "StationCraft";
    }

    public static class Item
    {
        public const string Drop = "ItemDrop";
    }

    public static class Enemy
    {
        public const string Spawn = "EnemySpawn";
        public const string Move = "EnemyMove";
        public const string Death = "EnemyDeath";
    }

    public static class Resource
    {
        public const string Spawn = "ResourceSpawn";
        public const string Harvested = "ResourceHarvested";
    }

    public static class GameState
    {
        public const string ScoreUpdate = "GameScoreUpdate";
        public const string TimeUpdate = "GameTimeUpdate";
    }

    public static class System
    {
        public const string Ping = "Ping";
        public const string Pong = "Pong";
        public const string Kick = "Kick";
    }
}