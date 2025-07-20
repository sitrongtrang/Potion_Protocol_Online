using System;
using Unity.VisualScripting;

public class NetworkInterpolationBuffer<TServerState>
    where TServerState : IServerStateSnapshot, IComparable<TServerState>
{
    private readonly MinHeap<TServerState> _serverStateBuffer;
    private readonly int _capacity;
    public NetworkInterpolationBuffer(int capacity)
    {
        _capacity = capacity;

        _serverStateBuffer = new();
    }

    public void Add(TServerState serverState)
    {
        if (_serverStateBuffer.Count >= _capacity) return;

        _serverStateBuffer.Add(serverState);
    }

    public bool Poll(int expectedSequence, out TServerState result)
    {
        while (_serverStateBuffer.Count != 0)
        {
            TServerState head = _serverStateBuffer.Peek();
            int seq = head.ServerSequence;
            if (seq < expectedSequence)
            {
                _serverStateBuffer.Pop();
            }
            else if (seq == expectedSequence)
            {
                result = _serverStateBuffer.Pop();
                return true;
            }
            else break;
            
        }
        result = default;
        return false;
    }

}