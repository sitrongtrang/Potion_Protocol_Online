using System;
using UnityEngine;

[Serializable]
public class PlayerPickupInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool PickupKeyDown;

    public PlayerPickupInputMessage() : base(NetworkMessageTypes.IngameInput.PlayerTryPickup) { }
}