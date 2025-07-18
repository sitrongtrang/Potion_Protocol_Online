using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public Vector2 Position;
    public string PlayerId;
}

class InputSnapshot
{
    public float horizontal, vertical;
    public long clientSendTime;
    public long clientEstimatedServerTime;
    public int clientTick;
}

public class PlayerStateSnapshot
{
    public PlayerState State;
    public long Timestamp;
}

public class PlayerInterpolationData
{
    public List<PlayerStateSnapshot> StateHistory = new List<PlayerStateSnapshot>();
    public const int MAX_HISTORY = 10;
    
    public void AddState(PlayerState state, long timestamp)
    {
        StateHistory.Add(new PlayerStateSnapshot { State = state, Timestamp = timestamp });
        
        while (StateHistory.Count > MAX_HISTORY)
        {
            StateHistory.RemoveAt(0);
        }
    }
}