using System.Collections.Generic;

public class GameStateUpdate : ServerMessage, IStateSnapshot
{
    public int ProcessedInputSequence;
    
    public PlayerState[] PlayerStates;
    public GameStateUpdate() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
}