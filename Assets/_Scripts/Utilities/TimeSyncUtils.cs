using System;

public static class TimeSyncUtils
{
    public static double GetUnixTimeSeconds()
    {
        return (DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
    }
    public static long GetUnixTimeMilliseconds()
    {
        return (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;
    }
}
