using System;
using System.Collections.Generic;

public class NetworkInterpolationBuffer<TServerState>
    where TServerState : IServerStateSnapshot, IComparable<TServerState>
{
    private readonly SortedList<int, TServerState> _serverStateBuffer;
    private readonly int _capacity;
    private readonly object _lock = new();

    private int _minTickToKeep = int.MinValue;
    public int Capacity => _capacity;
    // DEBUG
    public int? OldestTick
    {
        get
        {
            lock (_lock)
                return _serverStateBuffer.Count > 0 ? _serverStateBuffer.Keys[0] : (int?)null;
        }
    }

    public int? LatestTick
    {
        get
        {
            lock (_lock)
                return _serverStateBuffer.Count > 0 ? _serverStateBuffer.Keys[^1] : (int?)null;
        }
    }

    public NetworkInterpolationBuffer(int capacity)
    {
        _capacity = capacity;
        _serverStateBuffer = new();
    }

    public void SetMinTickToKeep(int tick)
    {
        lock (_lock)
        {
            _minTickToKeep = tick;
        }
    }

    public void Add(TServerState serverState)
    {
        int seq = serverState.ServerSequence;

        lock (_lock)
        {
            if (_serverStateBuffer.ContainsKey(seq))
                return;

            if (_serverStateBuffer.Count >= _capacity)
            {
                // Try to remove oldest if it's older than _minTickToKeep
                int oldestTick = _serverStateBuffer.Keys[0];
                if (oldestTick < _minTickToKeep)
                {
                    _serverStateBuffer.RemoveAt(0);
                }
                else
                {
                    // Can't safely evict anything; drop this snapshot
                    return;
                }
            }

            _serverStateBuffer.Add(seq, serverState);
        }
    }

    public TServerState Peek()
    {
        lock (_lock)
        {
            if (TryPeek(out var result))
                return result;
            return default;
        }
    }

    public bool Poll(int expectedSequence, out TServerState result)
    {
        lock (_lock)
        {
            result = default;

            while (TryPeek(out var head))
            {
                int seq = head.ServerSequence;

                if (seq < expectedSequence)
                {
                    TryPop(out _); // discard
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
    }

    public bool IsEmpty()
    {
        lock (_lock)
        {
            return _serverStateBuffer.Count == 0;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _serverStateBuffer.Clear();
        }
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
