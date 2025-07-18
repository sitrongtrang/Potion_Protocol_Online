using System;
using UnityEngine;

[Serializable]
public class PlayerMoveMessage : ServerMessage
{
    [JsonProperty("positionX")]
    public float PositionX;
    [JsonProperty("positionY")]
    public float PositionY;
    [JsonProperty("lastProcessedSeq")]
    public string LastProcessedInput;
    public bool RequiresCorrection;
    public bool IsDashing;

    public uint LastProcessedInputAsInt {
        get {
            if (long.TryParse(LastProcessedInput, out long longValue)) {
                // Handle Java's negative int -> C# uint conversion
                return (uint)longValue;
            }
            return 0; // Default fallback
        }
    }

    public PlayerMoveMessage() : base(NetworkMessageTypes.Server.Player.Movement) { }
}