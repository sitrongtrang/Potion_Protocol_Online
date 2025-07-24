using System;

[Serializable]
public class GameStatesUpdate : ServerMessage
{
    public GameStateUpdate[] GameStates;
    public GameStatesUpdate() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }

}

[Serializable]
public class GameStateUpdate : IStateSnapshot, IServerStateSnapshot
{
    public int ServerSequence;
    public int ProcessedInputSequence;
    public PlayerState[] PlayerStates;
    public StationState[] StationStates;
    public EnemyState[] EnemyStates;
    public ItemState[] ItemStates;
    public int CurrentGameTime;
    public int CurrentScore;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
    int IServerStateSnapshot.ServerSequence => ServerSequence;
}