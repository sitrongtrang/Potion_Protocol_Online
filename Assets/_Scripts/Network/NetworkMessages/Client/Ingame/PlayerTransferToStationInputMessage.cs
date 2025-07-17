using System;
using UnityEngine;

[Serializable]
public class PlayerTransferToStationInputMessage : ClientMessage
{
    public float CurretPositionX;
    public float CurrentPositionY;
    public int SelectedSlot;
    public bool PutToStationKeyDown;

    public PlayerTransferToStationInputMessage() : base(NetworkMessageTypes.Client.Ingame.TryTransferToStation) { }
}
