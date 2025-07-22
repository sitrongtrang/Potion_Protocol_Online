public class NetworkConstants
{
    public const float NET_TICK_MS = 33.3f;
    public const float SIM_TICK_MS = 16f;
    public const float NET_TICK_INTERVAL = NET_TICK_MS / 1000f;
    public const float SIM_TICK_INTERVAL = SIM_TICK_MS / 1000f;
    
    public const int NET_PRED_BUFFER_SIZE = 8;
    public const int NET_INTERPOLATION_BUFFER_SIZE = 5;
}