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
    where TInputMessage : IInputSnapshot
    where TSnapshot : IStateSnapshot
{
    void Simulate(TInputListener input, Func<TInputListener, TSnapshot> simulateAndReturnSnapshot);
    void Reconcile(
        TSnapshot serverSnapshot,
        Action<TSnapshot> applySnapshot,
        Func<TInputMessage, TSnapshot> simulateFromInput
    );
    void Reset();
}
public interface INetworkInterpolator<TClientState, TServerState>
    where TClientState : IServerStateSnapshot
    where TServerState : IServerStateSnapshot
{
    void Store(IReadOnlyList<TServerState> updates, Func<TServerState, int> findIdx);
    void Interpolate(Action<TClientState> applyState);
    void Reset();
}