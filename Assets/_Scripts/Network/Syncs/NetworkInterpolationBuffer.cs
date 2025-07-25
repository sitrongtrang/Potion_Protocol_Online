using System;
using System.Collections.Generic;

public class NetworkInterpolationBuffer<TServerState>
    where TServerState : IServerStateSnapshot, IComparable<TServerState>
{
    private readonly SortedList<int, TServerState> _serverStateBuffer;
    private readonly int _capacity;
    public int Capacity => _capacity;

    public NetworkInterpolationBuffer(int capacity)
    {
        _capacity = capacity;
        _serverStateBuffer = new();
    }

    public void Add(TServerState serverState)
    {
        int seq = serverState.ServerSequence;

        // Prevent duplicates
        if (_serverStateBuffer.ContainsKey(seq))
            return;

        if (_serverStateBuffer.Count >= _capacity)
            return;
            
        _serverStateBuffer.Add(serverState.ServerSequence, serverState);
    }

    public TServerState Peek()
    {
        if (TryPeek(out var result))
            return result;
        return default;
    }

    public bool Poll(int expectedSequence, out TServerState result)
    {
        result = default;

        while (TryPeek(out var head))
        {
            int seq = head.ServerSequence;

            if (seq < expectedSequence)
            {
                TryPop(out var dropped);
            }
            else if (seq == expectedSequence)
            {
                TryPop(out result);
                return true;
            }
            else
            {
                break;
            }
        }

        return false;
    }

    public void Clear()
    {
        _serverStateBuffer.Clear();
    }
    
    private bool TryPeek(out TServerState result)
    {
        if (_serverStateBuffer.Count > 0)
        {
            result = _serverStateBuffer.Values[0];
            return true;
        }
        result = default;
        return false;
    }

    private bool TryPop(out TServerState result)
    {
        if (_serverStateBuffer.Count > 0)
        {
            int firstKey = _serverStateBuffer.Keys[0];
            result = _serverStateBuffer[firstKey];
            _serverStateBuffer.RemoveAt(0);
            return true;
        }
        result = default;
        return false;
    }
}
