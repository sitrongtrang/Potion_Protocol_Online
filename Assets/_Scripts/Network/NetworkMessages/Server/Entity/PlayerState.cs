using Newtonsoft.Json;

public class PlayerState
{
    [JsonProperty("playerId")]
    public string PlayerId;
    [JsonProperty("positionX")]
    public float PositionX;
    [JsonProperty("positionY")]
    public float PositionY;
}