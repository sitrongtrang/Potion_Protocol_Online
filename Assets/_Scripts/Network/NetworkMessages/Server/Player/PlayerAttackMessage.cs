using System;
using UnityEngine;

[Serializable]
public class PlayerAttackMessage : ServerMessage
{
    public string[] TargetTypes;
    public int[] TargetIds;
    public float AttackDirectionX;
    public float AttackDirectionY;

    public PlayerAttackMessage() : base(NetworkMessageTypes.Server.Player.Attack) { }
}