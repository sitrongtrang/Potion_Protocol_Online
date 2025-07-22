using System;
using System.Collections.Generic;

public class NetworkInterpolationBuffer<TServerState>
    where TServerState : IServerStateSnapshot, IComparable<TServerState>
{
    private readonly ConcurrentMinHeap<TServerState> _serverStateBuffer;
    private readonly HashSet<int> _seenSequences;
    private readonly int _capacity;

    public NetworkInterpolationBuffer(int capacity)
    {
        _capacity = capacity;
        _serverStateBuffer = new();
        _seenSequences = new();
    }

    public void Add(TServerState serverState)
    {
        int seq = serverState.ServerSequence;

        // Prevent duplicates
        lock (_seenSequences)
        {
            if (_seenSequences.Contains(seq))
                return;

            if (_serverStateBuffer.Count >= _capacity)
                return;

            _seenSequences.Add(seq);
        }

        _serverStateBuffer.Add(serverState);
    }

    public TServerState Peek()
    {
        if (_serverStateBuffer.TryPeek(out var result))
            return result;

        return default;
    }

    public bool Poll(int expectedSequence, out TServerState result)
    {
        result = default;

        while (_serverStateBuffer.TryPeek(out var head))
        {
            int seq = head.ServerSequence;

            if (seq < expectedSequence)
            {
                _serverStateBuffer.TryPop(out var dropped);
                lock (_seenSequences)
                {
                    _seenSequences.Remove(dropped.ServerSequence);
                }
            }
            else if (seq == expectedSequence)
            {
                _serverStateBuffer.TryPop(out result);
                lock (_seenSequences)
                {
                    _seenSequences.Remove(expectedSequence);
                }
                return true;
            }
            else
            {
                break;
            }
        }

        return false;
    }
}
