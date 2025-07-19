using System.Collections.Generic;

public class GameStateUpdate : ServerMessage
{
    public PlayerState[] PlayerStates;
    public GameStateUpdate() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }
}