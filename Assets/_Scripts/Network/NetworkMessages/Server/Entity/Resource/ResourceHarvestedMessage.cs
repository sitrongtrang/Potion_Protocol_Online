using System;

[Serializable]
public class ResourceHarvestedMessage : ServerMessage
{
    public int ResourceId;
    public string HarvesterId;
    public string[] DroppedItems;

    public ResourceHarvestedMessage() : base(NetworkMessageTypes.Resource.Harvested) { }
}