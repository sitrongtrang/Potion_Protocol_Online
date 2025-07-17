using System;
using UnityEngine;

[Serializable]
public class EnemyMoveMessage : ServerMessage
{
    public int EnemyId;
    public float PositionX;
    public float PositionY;
    public float MovementDirectionX;
    public float MovementDirectionY;

    public EnemyMoveMessage() : base(NetworkMessageTypes.Server.Enemy.Move) { }
}