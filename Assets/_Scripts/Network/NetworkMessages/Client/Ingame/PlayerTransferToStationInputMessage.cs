using System;
using UnityEngine;

[Serializable]
public class PlayerTransferToStationInputMessage : ClientMessage
{
    public Vector2 CurrentPosition;
    public int SelectedSlot;
    public bool PutToStationKeyDown;

    public PlayerTransferToStationInputMessage() : base(NetworkMessageTypes.IngameInput.PlayerTryTransferToStation) { }
}
