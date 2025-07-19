using System.Collections.Generic;

public class NetworkPredictionBuffer<TInput, TState>
    where TInput : InputSnapshot
    where TState : StateSnapshot
{
    private readonly Queue<TInput> _inputBuffer;
    public TInput[] InputBufferAsArray => _inputBuffer.ToArray();

    private readonly Queue<TState> _stateBuffer;
    public TState[] StateBufferAsArray => _stateBuffer.ToArray();

    private readonly int _capacity;
    private int _currentInputSequence = -1;
    public NetworkPredictionBuffer(int capacity)
    {
        _capacity = capacity;
        _inputBuffer = new Queue<TInput>(capacity);
        _stateBuffer = new Queue<TState>(capacity);
    }

    public int GetCurrentInputSequence()
    {
        _currentInputSequence += 1;
        return _currentInputSequence;
        
    }

    public void EnqueueInput(TInput input)
    {
        if (_inputBuffer.Count >= _capacity)
            _inputBuffer.Dequeue();

        _inputBuffer.Enqueue(input);
    }

    public void EnqueueState(TState state)
    {
        if (_stateBuffer.Count >= _capacity)
            _stateBuffer.Dequeue();

        _stateBuffer.Enqueue(state);
    }
}
