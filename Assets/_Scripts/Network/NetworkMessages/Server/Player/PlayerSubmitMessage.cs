using System;
using UnityEngine;

[Serializable]
public class PlayerSubmitMessage : ServerMessage
{
    public string ItemId;
    public int SubmissionPointId;

    public PlayerSubmitMessage() : base(NetworkMessageTypes.Server.Player.Submit) { }
}

