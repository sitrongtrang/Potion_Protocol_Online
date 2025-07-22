using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerNetworkInterpolator : INetworkInterpolator<PlayerStateInterpolateData, GameStateUpdate>
{
    private NetworkInterpolationBuffer<PlayerStateInterpolateData> _buffer;
    private int _serverSequence = int.MaxValue;
    public PlayerNetworkInterpolator(int bufferSize)
    {
        _buffer = new(bufferSize);
    }
    public void Store(IReadOnlyList<GameStateUpdate> updates, Func<GameStateUpdate, int> findIdx)
    {
        bool inInitializing = _serverSequence == int.MaxValue;
        foreach (var update in updates)
        {
            int idx = findIdx(update);
            if (idx > -1)
            {
                if (inInitializing)
                {
                    if (update.ServerSequence < _serverSequence)
                    {
                        _serverSequence = update.ServerSequence;
                    }
                }
                else
                {
                    if (update.ServerSequence - _serverSequence > _buffer.Capacity)
                    {
                        _serverSequence = update.ServerSequence;
                    }
                }
                _buffer.Add(new PlayerStateInterpolateData()
                {
                    ServerSequence = update.ServerSequence,
                    PositionX = update.PlayerStates[idx].PositionX,
                    PositionY = update.PlayerStates[idx].PositionY
                });
            }
        }
    }
    public void Interpolate(Action<PlayerStateInterpolateData> applyState)
    {
        if (_buffer.Poll(_serverSequence, out PlayerStateInterpolateData result))
        {
            applyState(result);
            _serverSequence += 1;
        }
    }

    public void Reset()
    {
        _serverSequence = int.MaxValue;
        _buffer.Clear();
    }
}