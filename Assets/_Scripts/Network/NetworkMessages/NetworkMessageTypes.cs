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
            public const short RequestSpawn = 23;
        }

        public static class Ingame
        {
            public const short TryMove = 24;
            public const short TryPickup = 201;
            public const short TryDrop = 202;
            public const short TryTransferToStation = 203;
            public const short TryAttack = 204;
            public const short TryCraft = 205;
            public const short TrySubmit = 206;
            public const short TryCollide = 207;
        }

        public static class System
        {
            public const short Ping = 4;
        }
    }

    public static class Server
    {
        public static class System
        {
            public const short AuthSuccess = 0;
            public const short Pong = 20;
        }
        public static class Player
        {
            public const short Spawn = 4;
            public const short Connected = 1001;
            public const short Disconnected = 1002;
            public const short Movement = 5;
            public const short Inventory = 1004;
            public const short Attack = 1005;
            public const short Craft = 1006;
            public const short Submit = 1007;
            public const short Collide = 1008;
        }
        public static class Station
        {
            public const short Update = 1100;
            public const short Craft = 1101;
        }

        public static class Item
        {
            public const short Spawn = 1200;
            public const short Pickuped = 1201;
            public const short Drop = 1202;
            public const short Despawn = 1203;
        }

        public static class Enemy
        {
            public const short Spawn = 1300;
            public const short Move = 1301;
            public const short Death = 1302;
        }

        public static class Resource
        {
            public const short Spawn = 1400;
            public const short Harvested = 1401;
        }

        public static class GameState
        {
            public const short StateUpdate = 1500;
            public const short ScoreUpdate = 1501;
            public const short TimeUpdate = 1502;
        }
    }
}