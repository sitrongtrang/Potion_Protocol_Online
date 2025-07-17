using System;
using UnityEngine;

[Serializable]
public class EnemySpawnMessage : ServerMessage
{
    public int EnemyId;
    public string EnemyType;
    public int SpawnerId;
    public float PositionX;
    public float PositionY;
    public float PatrolCenterX;
    public float PatrolCenterY;
    

    public EnemySpawnMessage() : base(NetworkMessageTypes.Server.Enemy.Spawn) { }
}