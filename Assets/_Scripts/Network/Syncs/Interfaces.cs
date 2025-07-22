using System;
using System.Collections.Generic;

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
public interface INetworkSimulator<TInputListener, TInputMessage, TSnapshot>
{
    void Simulate(TInputListener input, Func<TInputListener, TSnapshot> simulateAndReturnSnapshot);
    void Reconcile(
        TSnapshot serverSnapshot,
        int processedInputSequence,
        Func<TSnapshot[]> getStateBuffer,
        Func<TInputMessage[]> getInputBuffer,
        Action<TSnapshot> applySnapshot,
        Func<TInputMessage, TSnapshot> simulateFromInput
    );
    void Reset();
}
public interface INetworkInterpolator<TState>
{
    void Store(IReadOnlyList<TState> updates);
    void Interpolate(Action<TState> applyState);
    void Reset();
}