using System;
using UnityEngine;

[Serializable]
public class EnemyDeathMessage : ServerMessage
{
    public int EnemyId;
    public string KillerId;
    public Vector2 DeathPosition;
    public string[] DroppedItems;

    public EnemyDeathMessage() : base(NetworkMessageTypes.Enemy.Death) { }
}