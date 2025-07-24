using System;

[Flags]
public enum EnemyStt
{
    None = 0,
    Return = 1 << 0,
    Patrol = 1 << 1,
    Idle = 1 << 2,
}
public class EnemyState
{
    public string EnemyId;
    public float PositionX;
    public float PositionY;
    public float CurrentHealth;
    public int CurrentState;
    public string ItemDrop;
}