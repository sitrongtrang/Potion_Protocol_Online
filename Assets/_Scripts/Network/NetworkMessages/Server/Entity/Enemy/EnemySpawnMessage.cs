using System;
using UnityEngine;

[Serializable]
public class EnemySpawnMessage : ServerMessage
{
    public int EnemyId;
    public string EnemyType;
    public int SpawnerId;
    public Vector2 Position;
    public Vector2[] PatrolPoints;

    public EnemySpawnMessage() : base(NetworkMessageTypes.Enemy.Spawn) { }
}