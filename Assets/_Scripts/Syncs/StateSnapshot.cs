using UnityEngine;
public class StateSnapshot
{
    public int ProcessedInputSequence;
}

public class PlayerSnapshot : StateSnapshot
{
    public Vector2 Position;
}