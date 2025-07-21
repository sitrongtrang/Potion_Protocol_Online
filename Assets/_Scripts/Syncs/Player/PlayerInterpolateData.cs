
using System;

public class PlayerStateInterpolateData : IServerStateSnapshot, IComparable<PlayerStateInterpolateData>
{
    public int ServerSequence;
    public float PositionX;
    public float PositionY;
    // public PlayerState State;

    int IServerStateSnapshot.ServerSequence => ServerSequence;

    public int CompareTo(PlayerStateInterpolateData other)
    {
        if (other == null) return 1;
        return ServerSequence.CompareTo(other.ServerSequence);
    }
}