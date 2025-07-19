using System.Collections.Generic;

public class GameStateUpdate : ServerMessage
{
    public int ProcessedInputSequence;
    
    public PlayerState[] PlayerStates;
    public GameStateUpdate() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }
}