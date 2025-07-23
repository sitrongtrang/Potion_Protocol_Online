using System;
using UnityEngine;

public class PlayerNetworkSimulator : INetworkSimulator<PlayerInputSnapshot, PlayerInputMessage, PlayerSnapshot>
{
    private readonly NetworkPredictionBuffer<PlayerInputMessage, PlayerSnapshot> _buffer;
    private bool _isReconciling = false;

    public PlayerInputMessage[] InputBufferAsArray => _buffer.InputBufferAsArray;
    public PlayerSnapshot[] StateBufferAsArray => _buffer.StateBufferAsArray;

    public PlayerNetworkSimulator(int bufferSize)
    {
        _buffer = new(bufferSize);
    }

    public void Simulate(PlayerInputSnapshot input, Func<PlayerInputSnapshot, PlayerSnapshot> applyInput)
    {
        if (_isReconciling) return;

        int sequence = _buffer.GetCurrentInputSequence();

        var message = new PlayerInputMessage(input)
        {
            InputSequence = sequence
        };

        var snapshot = applyInput(input);
        snapshot.ProcessedInputSequence = sequence;

        _buffer.EnqueueInput(message);
        _buffer.EnqueueState(snapshot);
    }

    public void Reconcile(
        PlayerSnapshot serverSnapshot,
        Action<PlayerSnapshot> resolveCannotReconcile,
        Func<PlayerSnapshot, PlayerSnapshot, bool> needReconciling,
        Action<PlayerSnapshot> applySnapshot,
        Func<PlayerInputMessage, PlayerSnapshot> simulateFromInput
    )
    {
        var stateSnapshots = (PlayerSnapshot[])_buffer.StateBufferAsArray.Clone();
        var inputSnapshots = (PlayerInputMessage[])_buffer.InputBufferAsArray.Clone();

        int index = -1;
        bool cannotReconcile = true;

        for (int i = 0; i < stateSnapshots.Length; i++)
        {
            if (stateSnapshots[i].ProcessedInputSequence == serverSnapshot.ProcessedInputSequence)
            {
                cannotReconcile = false;
                if (needReconciling(serverSnapshot, stateSnapshots[i]))
                    index = i;
                break;
            }
            else if (serverSnapshot.ProcessedInputSequence > stateSnapshots[i].ProcessedInputSequence)
            {
                cannotReconcile = false;
            }
        }

        if (cannotReconcile)
        {
            resolveCannotReconcile(serverSnapshot);
            return;
        }

        if (index == -1) return;

        _isReconciling = true;
        _buffer.ClearStateSnapshot();

        applySnapshot(serverSnapshot);

        _buffer.EnqueueState(serverSnapshot);

        for (int i = index + 1; i < inputSnapshots.Length; i++)
        {
            PlayerSnapshot newSnapshot = simulateFromInput(inputSnapshots[i]);

            applySnapshot(newSnapshot);
            _buffer.EnqueueState(newSnapshot);
        }

        _isReconciling = false;
    }

    public void Reset()
    {
        _isReconciling = false;
        _buffer.ClearStateSnapshot();
        _buffer.ClearInputSnapshot();
    }
}
