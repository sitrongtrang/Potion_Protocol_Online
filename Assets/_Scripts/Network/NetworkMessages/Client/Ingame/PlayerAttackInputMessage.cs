using System;
using UnityEngine;

[Serializable]
public class PlayerAttackInputMessage : ClientMessage
{
    public float CurretPositionX;
    public float CurrentPositionY;
    public Quaternion CurrentRotation;
    public bool AttackKeyDown;

    public PlayerAttackInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryAttack) { }
}