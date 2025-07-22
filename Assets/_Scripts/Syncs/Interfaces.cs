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
public interface INetworkSimulator<TInput, TSnapshot>
{
    void Simulate(TInput input, Action<TInput> applyInput);
    void Reconcile(
        TSnapshot serverSnapshot,
        int processedInputSequence,
        Func<TSnapshot[]> getStateBuffer,
        Func<TInput[]> getInputBuffer,
        Action<TSnapshot> applySnapshot,
        Func<TInput, TSnapshot> simulateFromInput
    );
    void EnqueueInput(TInput input);
    void Reset();
}
public interface INetworkInterpolator<TState>
{
    void Store(IReadOnlyList<TState> updates);
    void Interpolate(Action<TState> applyState);
    void Reset();
}