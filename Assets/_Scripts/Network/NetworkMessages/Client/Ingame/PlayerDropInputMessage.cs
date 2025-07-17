using System;
using UnityEngine;

[Serializable]
public class PlayerDropInputMessage : ClientMessage
{
    public float CurrentPositionX;
    public float CurrentPositionY;
    public int SelectedSlot;
    public bool DropKeyDown;

    public PlayerDropInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryDrop) { }
}