using System;
using UnityEngine;

[Serializable]
public class PlayerDropInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool DropKeyDown;

    public PlayerDropInputMessage() : base(NetworkMessageTypes.IngameInput.PlayerTryDrop) { }
}