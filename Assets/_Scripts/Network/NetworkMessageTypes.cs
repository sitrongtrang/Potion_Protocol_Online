public static class NetworkMessageTypes
{
    // Client -> Server Messages
    public static class Input
    {
        public const short PlayerMove = -1;
        public const short PlayerPickup = -1;
        public const short PlayerDrop = -1;
        public const short PlayerTransferToStation = -1;
        public const short PlayerAttack = -1;
        public const short PlayerCraft = -1;
        public const short PlayerSubmit = -1;
    }

    // Server -> Client Messages
    public static class Player
    {
        public const short Connected = -1;
        public const short Disconnected = -1;
        public const short Movement = -1;
        public const short Inventory = -1;
        public const short Attack = -1;
        public const short Craft = -1;
        public const short Submit = -1;
    }

    public static class Station
    {
        public const short Update = -1;
        public const short Craft = -1;
    }

    public static class Item
    {
        public const short Drop = -1;
    }

    public static class Enemy
    {
        public const short Spawn = -1;
        public const short Move = -1;
        public const short Death = -1;
    }

    public static class Resource
    {
        public const short Spawn = -1;
        public const short Harvested = -1;
    }

    public static class GameState
    {
        public const short ScoreUpdate = -1;
        public const short TimeUpdate = -1;
    }

    public static class System
    {
        public const short Ping = -1;
        public const short Pong = -1;
        public const short Kick = -1;
    }
}