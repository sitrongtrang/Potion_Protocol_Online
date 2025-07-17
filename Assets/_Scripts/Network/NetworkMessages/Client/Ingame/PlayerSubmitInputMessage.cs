using System;
using UnityEngine;

[Serializable]
public class PlayerSubmitInputMessage : ClientMessage
{
    public float CurrentPositionX;
    public float CurrentPositionY;
    public int SelectedSlot;
    public bool SubmitKeyDown;

    public PlayerSubmitInputMessage() : base(NetworkMessageTypes.Client.Ingame.TrySubmit) { }
}