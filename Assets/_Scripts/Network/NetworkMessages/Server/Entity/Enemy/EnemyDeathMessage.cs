using System;
using UnityEngine;

[Serializable]
public class EnemyDeathMessage : ServerMessage
{
    public int EnemyId;
    public string KillerId;
    public float DeathPositionX;
    public float DeathPositionY;
    public string[] DroppedItems;

    public EnemyDeathMessage() : base(NetworkMessageTypes.Server.Enemy.Death) { }
}