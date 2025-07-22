using System;
using System.Collections.Generic;
using Newtonsoft.Json;


public class GameStatesUpdate : ServerMessage
{
    public GameStateUpdate[] GameStates;
    public GameStatesUpdate() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }

}

public class GameStateUpdate : IStateSnapshot, IServerStateSnapshot
{
    public int ServerSequence;
    public int ProcessedInputSequence;
    public PlayerState[] PlayerStates;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
    int IServerStateSnapshot.ServerSequence => ServerSequence;
}