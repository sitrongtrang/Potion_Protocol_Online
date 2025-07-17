using System;
using UnityEngine;

[Serializable]
public class PlayerSpawnRequest : ClientMessage
{
    public PlayerSpawnRequest() : base(NetworkMessageTypes.Client.Pregame.RequestSpawn) { }
}