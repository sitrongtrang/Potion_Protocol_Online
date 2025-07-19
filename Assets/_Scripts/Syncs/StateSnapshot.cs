using UnityEngine;

public class PlayerSnapshot : IStateSnapshot
{
    public int ProcessedInputSequence;
    public Vector2 Position;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
}