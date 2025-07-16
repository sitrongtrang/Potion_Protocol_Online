public static class NetworkMessageTypes
{
    // Client -> Server Messages
    public static class Input
    {
        public const short PlayerTryAuth = 0;
        public const short PlayerTryMove = 4;
        public const short PlayerTryPickup = 2;
        public const short PlayerTryDrop = 3;
        public const short PlayerTryTransferToStation = 100;
        public const short PlayerTryAttack = 5;
        public const short PlayerTryCraft = 6;
        public const short PlayerTrySubmit = 7;
    }

    // Server -> Client Messages
    public static class Player
    {
        public const short Connected = 8;
        public const short Disconnected = 9;
        public const short Movement = 10;
        public const short Inventory = 11;
        public const short Attack = 12;
        public const short Craft = 13;
        public const short Submit = 14;
    }

    public static class Authorization
    {
        public const short AuthSuccess = 0;
    }

    public static class Station
    {
        public const short Update = 15;
        public const short Craft = 16;
    }

    public static class Item
    {
        public const short Drop = 17;
    }

    public static class Enemy
    {
        public const short Spawn = 18;
        public const short Move = 19;
        public const short Death = 20;
    }

    public static class Resource
    {
        public const short Spawn = 21;
        public const short Harvested = 22;
    }

    public static class GameState
    {
        public const short ScoreUpdate = 23;
        public const short TimeUpdate = 24;
    }

    public static class System
    {
        public const short Ping = 25;
        public const short Pong = 26;
        public const short Kick = 27;
    }
}