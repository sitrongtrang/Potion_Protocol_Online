public static class NetworkMessageTypes
{
    public static class Client
    {
        public static class Authentication
        {
            public const short TryAuth = 0;
            public const short TryReconnect = 1;

        }

        public static class Pregame
        {
            public const short RequestSpawn = 900;
        }

        public static class Ingame
        {
            public const short TryMove = 4;
            public const short TryPickup = 2;
            public const short TryDrop = 3;
            public const short TryTransferToStation = 100;
            public const short TryAttack = 5;
            public const short TryCraft = 6;
            public const short TrySubmit = 7;
            public const short TryCollide = 101;
        }

        public static class System
        {
            public const short Ping = 25;
        }
    }

    public static class Server
    {
        public static class System
        {
            public const short AuthSuccess = 0;
            public const short Pong = 26;
        }
        public static class Player
        {
            public const short Spawn = 1000;
            public const short Connected = 8;
            public const short Disconnected = 9;
            public const short Movement = 10;
            public const short Inventory = 11;
            public const short Attack = 12;
            public const short Craft = 13;
            public const short Submit = 14;
            public const short Collide = 102;
        }
        public static class Station
        {
            public const short Update = 15;
            public const short Craft = 16;
        }

        public static class Item
        {
            public const short Spawn = 600;
            public const short Pickuped = 500;
            public const short Drop = 17;
            public const short Despawn = 400;
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
    }
}