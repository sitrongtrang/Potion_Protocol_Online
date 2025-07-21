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
            public const short Input = 299;
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
        }

        public static class GameState
        {
            public const short StateUpdate = 5;
            public const short ScoreUpdate = 1501;
            public const short TimeUpdate = 1502;
        }
    }
}