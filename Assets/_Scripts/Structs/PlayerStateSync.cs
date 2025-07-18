using System;
using UnityEngine;

public class PlayerState
{
    public Vector2 Position;
    public string PlayerId;
}

class InputSnapshot {
    public float horizontal, vertical;
    public long clientSendTime;
    public int clientTick;
}