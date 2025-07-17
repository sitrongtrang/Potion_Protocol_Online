using System;
using UnityEngine;

[Serializable]
public class PlayerSubmitInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool SubmitKeyDown;

    public PlayerSubmitInputMessage() : base(NetworkMessageTypes.Client.Ingame.TrySubmit) { }
}