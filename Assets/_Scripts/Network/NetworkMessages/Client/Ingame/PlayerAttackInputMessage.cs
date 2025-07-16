using System;
using UnityEngine;

[Serializable]
public class PlayerAttackInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public Quaternion CurrentRotation;
    public bool AttackKeyDown;

    public PlayerAttackInputMessage() : base(NetworkMessageTypes.IngameInput.PlayerTryAttack) { }
}