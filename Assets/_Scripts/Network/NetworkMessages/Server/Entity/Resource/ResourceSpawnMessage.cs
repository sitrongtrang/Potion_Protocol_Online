using System;
using UnityEngine;

[Serializable]
public class ResourceSpawnMessage : ServerMessage
{
    public int ResourceId;
    public string ResourceType;
    public Vector2 Position;

    public ResourceSpawnMessage() : base(NetworkMessageTypes.Resource.Spawn) { }
}