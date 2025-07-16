using System;
using UnityEngine;

[Serializable]
public class PlayerAttackMessage : ServerMessage
{
    public string[] TargetTypes;
    public int[] TargetIds;
    public Vector2 AttackDirection;

    public PlayerAttackMessage() : base(NetworkMessageTypes.Player.Attack) { }
}