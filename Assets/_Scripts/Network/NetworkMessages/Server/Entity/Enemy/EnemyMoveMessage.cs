using System;
using UnityEngine;

[Serializable]
public class EnemyMoveMessage : ServerMessage
{
    public int EnemyId;
    public Vector2 Position;
    public Vector2 MovementDirection;

    public EnemyMoveMessage() : base(NetworkMessageTypes.Server.Enemy.Move) { }
}