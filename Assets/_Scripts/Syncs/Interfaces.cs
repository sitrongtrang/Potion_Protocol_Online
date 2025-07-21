using System;

public interface IInputSnapshot
{
    public int InputSequence { get; }
}
public interface IStateSnapshot
{
    public int ProcessedInputSequence { get; }
}
public interface IServerStateSnapshot
{
    public int ServerSequence { get; }

    
}