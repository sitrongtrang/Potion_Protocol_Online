public class PlayerState : IStateSnapshot
{
    public string PlayerId;
    public int ProcessedInputSequence;
    public float PositionX;
    public float PositionY;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
}