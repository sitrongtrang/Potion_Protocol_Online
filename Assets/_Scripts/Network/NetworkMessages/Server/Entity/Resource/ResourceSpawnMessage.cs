using System;
using UnityEngine;

[Serializable]
public class ResourceSpawnMessage : ServerMessage
{
    public int ResourceId;
    public string ResourceType;
    public float PositionX;
    public float PositionY;

    public ResourceSpawnMessage() : base(NetworkMessageTypes.Server.Resource.Spawn) { }
}