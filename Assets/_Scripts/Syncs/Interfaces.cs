public interface IInputSnapshot
{
    public int InputSequence { get; }
}
public interface IStateSnapshot
{
    public int ProcessedInputSequence { get; }
}