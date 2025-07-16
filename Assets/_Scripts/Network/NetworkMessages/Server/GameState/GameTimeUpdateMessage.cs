using System;

[Serializable]
public class GameTimeUpdateMessage : ServerMessage
{
    public float RemainingGameTime;
    public int CurrentWave;

    public GameTimeUpdateMessage() : base(NetworkMessageTypes.GameState.TimeUpdate) { }
}