using System;

[Serializable]
public class GameScoreUpdateMessage : ServerMessage
{
    public int ScoreChange;
    public int NewTotalScore;

    public GameScoreUpdateMessage() : base(NetworkMessageTypes.GameState.ScoreUpdate) { }
}