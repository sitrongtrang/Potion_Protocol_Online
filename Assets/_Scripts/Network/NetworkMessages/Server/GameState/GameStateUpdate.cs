using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// public class GameStateWrapper : ServerMessage
// {
//     [JsonProperty("gameStates")]
//     public GameStatesUpdate gameStatesUpdate;
//     public GameStateWrapper() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }
// }

public class OuterGameStatesWrapper
{
    [JsonProperty("gameStates")]
    public string GameStatesJson { get; set; }
}

public class GameStatesUpdate : ServerMessage
{
    [JsonProperty("gameStatesSnapshot")]
    public List<GameStateUpdate> GameStates;
    public GameStatesUpdate() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }

}

public class GameStateUpdate : IStateSnapshot, IServerStateSnapshot
{
    [JsonProperty("serverSequence")]
    public int ServerSequence;
    [JsonProperty("processedInputSequence")]
    public int ProcessedInputSequence;
    [JsonProperty("playersSnapshot")]
    public PlayerState[] PlayerStates;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
    int IServerStateSnapshot.ServerSequence => ServerSequence;
}

public class PlayerStateInterpolateData : IServerStateSnapshot, IComparable<PlayerStateInterpolateData>
{
    public int ServerSequence;
    public float PositionX;
    public float PositionY;
    // public PlayerState State;

    int IServerStateSnapshot.ServerSequence => ServerSequence;

    public int CompareTo(PlayerStateInterpolateData other)
    {
        if (other == null) return 1;
        return ServerSequence.CompareTo(other.ServerSequence);
    }
}