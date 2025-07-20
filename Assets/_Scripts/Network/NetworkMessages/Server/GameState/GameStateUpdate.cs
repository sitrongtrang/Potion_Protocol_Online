using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class GameStatesUpdate : ServerMessage
{
    [JsonProperty("gameStates")]
    public GameStateUpdate[] GameStates;
    public GameStatesUpdate() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }

}

public class GameStateUpdate : IStateSnapshot, IServerStateSnapshot, IComparable<GameStateUpdate>
{
    [JsonProperty("serverSequence")]
    public int ServerSequence;
    [JsonProperty("processedInputSequence")]
    public int ProcessedInputSequence;
    [JsonProperty("playersSnapshot")]
    public PlayerState[] PlayerStates;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
    int IServerStateSnapshot.ServerSequence => ServerSequence;

    public int CompareTo(GameStateUpdate other)
    {
        if (other == null) return 1;
        return ProcessedInputSequence.CompareTo(other.ProcessedInputSequence);
    }
}